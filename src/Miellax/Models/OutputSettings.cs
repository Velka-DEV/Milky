﻿using Miellax.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Miellax.Models
{
    public class OutputSettings
    {
        /// <summary>
        /// Directory to output results to
        /// </summary>
        public string OutputDirectory { get; set; } = Path.Combine("Results", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM dd, yyyy — HH.mm.ss")));

        /// <summary>
        /// Whether to output invalids or not
        /// Warning: This may slow down your check substantially
        /// </summary>
        public bool OutputInvalids { get; set; } = false;

        /// <summary>
        /// Whether to display unknown's in console
        /// </summary>
        public bool OutputUnknowns { get; set; } = false;

        /// <summary>
        /// Whether to display banned's in console
        /// </summary>
        public bool OutputLockeds { get; set; } = false;

        /// <summary>
        /// Whether to display free's in console
        /// </summary>
        public bool DisplayFrees { get; set; } = true;

        /// <summary>
        /// Separator that's going to be used to separate the combo and the capture, as well as each capture
        /// </summary>
        public string CaptureSeparator { get; set; } = " | ";

        /// <summary>
        /// Console output color for <see cref="ComboResult.Hit"/>
        /// </summary>
        public ConsoleColor HitColor { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// Console output color for <see cref="ComboResult.Free"/>
        /// </summary>
        public ConsoleColor FreeColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Console output color for <see cref="ComboResult.Unknown"/>
        /// </summary>
        public ConsoleColor UnknownColor { get; set; } = ConsoleColor.DarkRed;
        
        /// <summary>
        /// Console output color for <see cref="ComboResult.Banned"/>
        /// </summary>
        public ConsoleColor BannedColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Console output color for <see cref="ComboResult.Invalid"/>
        /// </summary>
        public ConsoleColor InvalidColor { get; set; } = ConsoleColor.Red;

        /// <summary>
        /// Merge outputs to global results file
        /// </summary>
        public bool GlobalOutput { get; set; } = false;

        /// <summary>
        /// Custom <see cref="ConsoleColor"/> output if capture conditional check is true, first match is used
        /// </summary>
        public Dictionary<ConsoleColor, KeyValuePair<string, Predicate<object>>> CustomColors { get; set; } = new Dictionary<ConsoleColor, KeyValuePair<string, Predicate<object>>>();
    }
}
