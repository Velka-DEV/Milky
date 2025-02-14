﻿using Miellax.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miellax
{
    public class ConsoleManager
    {
        private readonly Checker _checker;

        public ConsoleManager(Checker checker)
        {
            _checker = checker;
        }

        /// <summary>
        /// Will start updating your <see cref="Console.Title"/> using your <see cref="Checker"/> statistics
        /// </summary>
        /// <param name="updateInterval">Interval between title updates</param>
        /// <param name="showFree">Whether you want <see cref="ComboResult.Free"/> to be shown</param>
        /// <param name="showPercentages">Whether you want some cool percentages to be shown</param>
        /// <param name="prefix">Prefix to add to title, can be useful as something like "Miellax Checker v1.0.0 by Laiteux — "</param>
        /// <param name="suffix">Suffix to add to title, can be useful for uh idk</param>
        public async Task StartUpdatingTitleAsync(TimeSpan updateInterval, bool showFree = true, bool showPercentages = true, string prefix = null, string suffix = null)
        {
            while (true)
            {
                var title = new StringBuilder().Append(prefix).Append(_checker.Info.Status);

                if (_checker.Info.Status != CheckerStatus.Idle)
                {
                    var checkStats = new List<string>()
                    {
                        "Checked: " + ((double)_checker.Info.Checked.Count).ToString("N0"),
                        "Hits: " + ((double)_checker.Info.Hits).ToString("N0")
                    };

                    if (showFree)
                    {
                        checkStats.Add("Free: " + ((double)_checker.Info.Free).ToString("N0"));
                    }

                    if (showPercentages)
                    {
                        if (_checker.Info.Status != CheckerStatus.Done)
                        {
                            checkStats[0] += $" ({(double)_checker.Info.Checked.Count / _checker.Info.Credentials:P2})";
                        }

                        checkStats[1] += $" ({(double)_checker.Info.Hits / _checker.Info.Checked.Count:P2})";

                        if (showFree)
                        {
                            checkStats[2] += $" ({(double)_checker.Info.Free / _checker.Info.Checked.Count:P2})";
                        }
                    }

                    var runStats = new List<string>()
                    {
                        "Elapsed: " + _checker.Info.Elapsed
                    };

                    if (_checker.Info.Status != CheckerStatus.Done)
                    {
                        checkStats.Insert(1, "Left: " + ((double)(_checker.Info.Credentials - _checker.Info.Checked.Count)).ToString("N0"));

                        if (_checker.Info.Hits != 0)
                        {
                            checkStats.Insert(3, "Estimated: " + _checker.Info.EstimatedHits.ToString("N0"));
                        }

                        if (_checker.Info.Retries != 0)
                        {
                            checkStats.Insert(3, "Retries: " + _checker.Info.Retries.ToString("N0"));
                        }

                        if (_checker.Info.Status != CheckerStatus.Paused)
                        {
                            runStats.Insert(0, "CPM: " + ((double)_checker.Info.Cpm).ToString("N0"));
                            runStats.Insert(2, "Remaining: " + (_checker.Info.Remaining?.ToString() ?? "?"));
                        }
                    }

                    title
                        .Append(" | ").AppendJoin(" — ", checkStats)
                        .Append(" | ").AppendJoin(" — ", runStats);
                }

                Console.Title = title.Append(suffix).ToString();

                if (_checker.Info.Status == CheckerStatus.Done)
                {
                    break;
                }

                await Task.Delay(updateInterval).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Will start listening to user keys and do actions associated with them when pressed
        /// </summary>
        /// <param name="pause"><see cref="ConsoleKey"/> for <see cref="Checker.Pause"/>, <see cref="null"/> to disable</param>
        /// <param name="resume"><see cref="ConsoleKey"/> for <see cref="Checker.Resume"/>, <see cref="null"/> to disable</param>
        /// /// <param name="saveUnchecked"><see cref="ConsoleKey"/> for saving unchecked combos, <see cref="null"/> to disable</param>
        /// <param name="abort"><see cref="ConsoleKey"/> for <see cref="Checker.Abort"/>, <see cref="null"/> to disable</param>
        public async Task StartListeningKeysAsync(ConsoleKey? pause = ConsoleKey.P, ConsoleKey? resume = ConsoleKey.R, ConsoleKey? saveUnchecked = ConsoleKey.S, ConsoleKey? abort = null)
        {
            while (_checker.Info.Status != CheckerStatus.Done)
            {
                if (!Console.KeyAvailable)
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    continue;
                }

                if (_checker.Info.Status == CheckerStatus.Idle)
                {
                    continue;
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                if ((pause != null && key == pause) || (resume != null && key == resume))
                {
                    if (_checker.Info.Status == CheckerStatus.Running)
                    {
                        _checker.Pause();

                        lock (_checker.Info.Locker)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{Environment.NewLine}Checker paused, press \"{resume}\" to resume...{Environment.NewLine}");
                        }
                    }
                    else if (_checker.Info.Status == CheckerStatus.Paused)
                    {
                        lock (_checker.Info.Locker)
                        {
                            if (_checker.Info.LastHit > _checker.Info.LastPause)
                            {
                                Console.WriteLine();
                            }

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"Checker resumed! Pause duration: {TimeSpan.FromSeconds((int)_checker.Resume().TotalSeconds)}{Environment.NewLine}");
                        }
                    }
                }
                else if (saveUnchecked != null && key == saveUnchecked)
                {
                    lock (_checker.Info.Locker)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{Environment.NewLine}Saving unchecked combos, please wait...{Environment.NewLine}");
                    }

                    var saveStart = DateTime.Now;

                    int @unchecked = _checker.SaveUnchecked();

                    lock (_checker.Info.Locker)
                    {
                        if (_checker.Info.LastHit > saveStart)
                        {
                            Console.WriteLine();
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"Saved {@unchecked:N0} unchecked combos!{Environment.NewLine}");
                    }
                }
                else if (abort != null && key == abort)
                {
                    _checker.Abort();
                }
            }
        }
    }
}
