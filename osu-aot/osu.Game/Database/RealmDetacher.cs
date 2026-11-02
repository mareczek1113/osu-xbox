// Xbox port: hand-written replacement for the AutoMapper-based Detach / CopyChangesToRealm in RealmObjectExtensions.
// AutoMapper builds its maps with expression trees and reflection and fails under Native AOT.
// The copied members mirror the AutoMapper configuration: every persisted property with a public setter
// ([Ignored], [Backlink] and get-only members excluded). Lists are copied item by item.

using System;
using System.Diagnostics;
using osu.Framework.Input.Bindings;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Collections;
using osu.Game.Input.Bindings;
using osu.Game.Models;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using osu.Game.Skinning;
using Realms;

namespace osu.Game.Database
{
    internal static class RealmDetacher
    {
        /// <summary>Detach a managed object. Mirrors the old AutoMapper `mapper` / `beatmap_set_mapper` behaviour.</summary>
        public static T Detach<T>(T item) where T : RealmObjectBase
        {
            RealmObjectBase result = item switch
            {
                BeatmapSetInfo set => copySet(set, includeFiles: false, replaceSource: null, replaceWith: null),
                BeatmapInfo beatmap => copyBeatmapWithSet(beatmap),
                BeatmapMetadata metadata => copyMetadata(metadata),
                BeatmapDifficulty difficulty => copyDifficulty(difficulty),
                BeatmapUserSettings settings => copyUserSettings(settings),
                RealmUser user => copyUser(user),
                RealmFile file => copyFile(file),
                RealmNamedFileUsage usage => copyUsage(usage),
                RulesetInfo ruleset => copyRuleset(ruleset),
                ScoreInfo score => copyScore(score),
                SkinInfo skin => copySkin(skin),
                BeatmapCollection collection => copyCollection(collection),
                RealmKeyBinding binding => copyKeyBinding(binding),
                _ => unsupported(item),
            };

            return (T)(object)result;
        }

        private static RealmObjectBase unsupported(RealmObjectBase item)
        {
            // AutoMapper had no map for other types either (it would have thrown); keep the managed instance and log.
            Logger.Log($"Detach is not implemented for {item.GetType().Name}; returning the managed instance.", LoggingTarget.Database, LogLevel.Important);
            return item;
        }

        // --- embedded / leaf types -------------------------------------------------------------------------------

        private static BeatmapDifficulty copyDifficulty(BeatmapDifficulty s) => new BeatmapDifficulty
        {
            DrainRate = s.DrainRate,
            CircleSize = s.CircleSize,
            OverallDifficulty = s.OverallDifficulty,
            ApproachRate = s.ApproachRate,
            SliderMultiplier = s.SliderMultiplier,
            SliderTickRate = s.SliderTickRate,
        };

        private static BeatmapUserSettings copyUserSettings(BeatmapUserSettings s) => new BeatmapUserSettings { Offset = s.Offset };

        private static RealmUser copyUser(RealmUser s) => new RealmUser
        {
            OnlineID = s.OnlineID,
            Username = s.Username,
            CountryString = s.CountryString,
        };

        private static RealmFile copyFile(RealmFile s) => new RealmFile { Hash = s.Hash };

        private static RealmNamedFileUsage copyUsage(RealmNamedFileUsage s) => new RealmNamedFileUsage(copyFile(s.File), s.Filename);

        private static RulesetInfo copyRuleset(RulesetInfo s) => new RulesetInfo
        {
            ShortName = s.ShortName,
            OnlineID = s.OnlineID,
            Name = s.Name,
            InstantiationInfo = s.InstantiationInfo,
            LastAppliedDifficultyVersion = s.LastAppliedDifficultyVersion,
            Available = s.Available,
        };

        private static BeatmapMetadata copyMetadata(BeatmapMetadata s)
        {
            var d = new BeatmapMetadata(copyUser(s.Author))
            {
                Title = s.Title,
                TitleUnicode = s.TitleUnicode,
                Artist = s.Artist,
                ArtistUnicode = s.ArtistUnicode,
                Source = s.Source,
                Tags = s.Tags,
                PreviewTime = s.PreviewTime,
                AudioFile = s.AudioFile,
                BackgroundFile = s.BackgroundFile,
            };

            foreach (string tag in s.UserTags)
                d.UserTags.Add(tag);

            return d;
        }

        // --- beatmaps ----------------------------------------------------------------------------------------------

        /// <summary>Copies a beatmap without its set (BeatmapSet stays null).</summary>
        private static BeatmapInfo copyBeatmapShallow(BeatmapInfo s)
        {
            var d = new BeatmapInfo(copyRuleset(s.Ruleset), copyDifficulty(s.Difficulty), copyMetadata(s.Metadata))
            {
                ID = s.ID,
                DifficultyName = s.DifficultyName,
                UserSettings = copyUserSettings(s.UserSettings),
                StatusInt = s.StatusInt,
                OnlineID = s.OnlineID,
                Length = s.Length,
                BPM = s.BPM,
                Hash = s.Hash,
                StarRating = s.StarRating,
                MD5Hash = s.MD5Hash,
                OnlineMD5Hash = s.OnlineMD5Hash,
                LastLocalUpdate = s.LastLocalUpdate,
                LastOnlineUpdate = s.LastOnlineUpdate,
                Hidden = s.Hidden,
                EndTimeObjectCount = s.EndTimeObjectCount,
                TotalObjectCount = s.TotalObjectCount,
                LastPlayed = s.LastPlayed,
                BeatDivisor = s.BeatDivisor,
                EditorTimestamp = s.EditorTimestamp,
            };

            return d;
        }

        /// <summary>
        /// Detach a beatmap together with its set. The result is a consistent graph: the returned beatmap is the instance inside
        /// its set's beatmap list, and every beatmap in the set points back at the copied set.
        /// </summary>
        private static BeatmapInfo copyBeatmapWithSet(BeatmapInfo s)
        {
            var d = copyBeatmapShallow(s);

            if (s.BeatmapSet != null)
                d.BeatmapSet = copySet(s.BeatmapSet, includeFiles: true, replaceSource: s, replaceWith: d);

            return d;
        }

        private static BeatmapSetInfo copySet(BeatmapSetInfo s, bool includeFiles, BeatmapInfo? replaceSource, BeatmapInfo? replaceWith)
        {
            var d = new BeatmapSetInfo(null)
            {
                ID = s.ID,
                OnlineID = s.OnlineID,
                DateAdded = s.DateAdded,
                DateSubmitted = s.DateSubmitted,
                DateRanked = s.DateRanked,
                StatusInt = s.StatusInt,
                DeletePending = s.DeletePending,
                Hash = s.Hash,
                Protected = s.Protected,
            };

            if (includeFiles)
            {
                foreach (var file in s.Files)
                    d.Files.Add(copyUsage(file));
            }

            foreach (var beatmap in s.Beatmaps)
            {
                var copy = replaceSource != null && replaceWith != null && beatmap.Equals(replaceSource) ? replaceWith : copyBeatmapShallow(beatmap);
                copy.BeatmapSet = d;
                d.Beatmaps.Add(copy);
            }

            return d;
        }

        // --- other top-level models --------------------------------------------------------------------------------

        private static ScoreInfo copyScore(ScoreInfo s)
        {
            var d = new ScoreInfo(s.BeatmapInfo == null ? null : copyBeatmapWithSet(s.BeatmapInfo), copyRuleset(s.Ruleset), copyUser(s.RealmUser))
            {
                ID = s.ID,
                ClientVersion = s.ClientVersion,
                BeatmapHash = s.BeatmapHash,
                Hash = s.Hash,
                DeletePending = s.DeletePending,
                TotalScore = s.TotalScore,
                TotalScoreWithoutMods = s.TotalScoreWithoutMods,
                TotalScoreVersion = s.TotalScoreVersion,
                LegacyTotalScore = s.LegacyTotalScore,
                BackgroundReprocessingFailed = s.BackgroundReprocessingFailed,
                MaxCombo = s.MaxCombo,
                Accuracy = s.Accuracy,
                Date = s.Date,
                PP = s.PP,
                OnlineID = s.OnlineID,
                LegacyOnlineID = s.LegacyOnlineID,
                ModsJson = s.ModsJson,
                StatisticsJson = s.StatisticsJson,
                MaximumStatisticsJson = s.MaximumStatisticsJson,
                RankInt = s.RankInt,
                Combo = s.Combo,
                IsLegacyScore = s.IsLegacyScore,
            };

            if (s.BeatmapInfo == null)
                d.BeatmapInfo = null;

            foreach (var file in s.Files)
                d.Files.Add(copyUsage(file));

            foreach (int pause in s.Pauses)
                d.Pauses.Add(pause);

            return d;
        }

        private static SkinInfo copySkin(SkinInfo s)
        {
            var d = new SkinInfo(s.Name, s.Creator, s.InstantiationInfo)
            {
                ID = s.ID,
                Hash = s.Hash,
                Protected = s.Protected,
                DeletePending = s.DeletePending,
            };

            foreach (var file in s.Files)
                d.Files.Add(copyUsage(file));

            return d;
        }

        private static BeatmapCollection copyCollection(BeatmapCollection s)
        {
            var d = new BeatmapCollection(s.Name, null)
            {
                ID = s.ID,
                LastModified = s.LastModified,
            };

            foreach (string hash in s.BeatmapMD5Hashes)
                d.BeatmapMD5Hashes.Add(hash);

            return d;
        }

        private static RealmKeyBinding copyKeyBinding(RealmKeyBinding s) =>
            new RealmKeyBinding(s.ActionInt, new KeyCombination(s.KeyCombinationString), s.RulesetName, s.Variant) { ID = s.ID };

        // --- copy changes back to realm (old `write_mapper`) --------------------------------------------------------

        public static void CopyChangesToRealm<T>(T source, T destination) where T : RealmObjectBase
        {
            switch (source)
            {
                case BeatmapSetInfo set:
                    copyChangesSet(set, (BeatmapSetInfo)(object)destination);
                    break;

                case BeatmapInfo beatmap:
                    copyChangesBeatmap(beatmap, (BeatmapInfo)(object)destination);
                    break;

                case BeatmapMetadata metadata:
                    copyChangesMetadata(metadata, (BeatmapMetadata)(object)destination);
                    break;

                case BeatmapDifficulty difficulty:
                    copyChangesDifficulty(difficulty, (BeatmapDifficulty)(object)destination);
                    break;

                case RealmUser user:
                    copyChangesUser(user, (RealmUser)(object)destination);
                    break;

                default:
                    throw new NotSupportedException($"CopyChangesToRealm is not implemented for {source.GetType().Name}.");
            }
        }

        private static void copyChangesUser(RealmUser s, RealmUser d)
        {
            d.OnlineID = s.OnlineID;
            d.Username = s.Username;
            d.CountryString = s.CountryString;
        }

        private static void copyChangesDifficulty(BeatmapDifficulty s, BeatmapDifficulty d)
        {
            d.DrainRate = s.DrainRate;
            d.CircleSize = s.CircleSize;
            d.OverallDifficulty = s.OverallDifficulty;
            d.ApproachRate = s.ApproachRate;
            d.SliderMultiplier = s.SliderMultiplier;
            d.SliderTickRate = s.SliderTickRate;
        }

        private static void copyChangesMetadata(BeatmapMetadata s, BeatmapMetadata d)
        {
            d.Title = s.Title;
            d.TitleUnicode = s.TitleUnicode;
            d.Artist = s.Artist;
            d.ArtistUnicode = s.ArtistUnicode;
            d.Source = s.Source;
            d.Tags = s.Tags;
            d.PreviewTime = s.PreviewTime;
            d.AudioFile = s.AudioFile;
            d.BackgroundFile = s.BackgroundFile;

            copyChangesUser(s.Author, d.Author);
        }

        private static void copyChangesBeatmap(BeatmapInfo s, BeatmapInfo d)
        {
            d.DifficultyName = s.DifficultyName;
            d.StatusInt = s.StatusInt;
            d.OnlineID = s.OnlineID;
            d.Length = s.Length;
            d.BPM = s.BPM;
            d.Hash = s.Hash;
            d.StarRating = s.StarRating;
            d.MD5Hash = s.MD5Hash;
            d.OnlineMD5Hash = s.OnlineMD5Hash;
            d.LastLocalUpdate = s.LastLocalUpdate;
            d.LastOnlineUpdate = s.LastOnlineUpdate;
            d.Hidden = s.Hidden;
            d.EndTimeObjectCount = s.EndTimeObjectCount;
            d.TotalObjectCount = s.TotalObjectCount;
            d.LastPlayed = s.LastPlayed;
            d.BeatDivisor = s.BeatDivisor;
            d.EditorTimestamp = s.EditorTimestamp;

            d.Ruleset = d.Realm!.Find<RulesetInfo>(s.Ruleset.ShortName)!;
            copyChangesDifficulty(s.Difficulty, d.Difficulty);
            copyChangesMetadata(s.Metadata, d.Metadata);
        }

        private static void copyChangesSet(BeatmapSetInfo s, BeatmapSetInfo d)
        {
            d.OnlineID = s.OnlineID;
            d.DateAdded = s.DateAdded;
            d.DateSubmitted = s.DateSubmitted;
            d.DateRanked = s.DateRanked;
            d.StatusInt = s.StatusInt;
            d.DeletePending = s.DeletePending;
            d.Hash = s.Hash;
            d.Protected = s.Protected;

            foreach (var beatmap in s.Beatmaps)
            {
                // Importantly, search all of realm for the beatmap (not just the set's beatmaps).
                // It may have gotten detached, and if that's the case let's use this opportunity to fix things up.
                var existingBeatmap = d.Realm!.Find<BeatmapInfo>(beatmap.ID);

                if (existingBeatmap != null)
                {
                    // As above, reattach if it happens to not be in the set's beatmaps.
                    if (!d.Beatmaps.Contains(existingBeatmap))
                    {
                        Debug.Fail("Beatmaps should never become detached under normal circumstances. If this ever triggers, it should be investigated further.");
                        Logger.Log("WARNING: One of the difficulties in a beatmap was detached from its set. Please save a copy of logs and report this to devs.", LoggingTarget.Database,
                            LogLevel.Important);
                        d.Beatmaps.Add(existingBeatmap);
                    }

                    copyChangesBeatmap(beatmap, existingBeatmap);
                }
                else
                {
                    var newBeatmap = new BeatmapInfo
                    {
                        ID = beatmap.ID,
                        BeatmapSet = d,
                        Ruleset = d.Realm.Find<RulesetInfo>(beatmap.Ruleset.ShortName)!
                    };

                    d.Beatmaps.Add(newBeatmap);
                    copyChangesBeatmap(beatmap, newBeatmap);
                }
            }
        }
    }
}
