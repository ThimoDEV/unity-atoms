///MIT License
///Copyright(c) 2019 InnoGames GmbH

using UnityEngine;

namespace UnityAtoms.Editor
{
    /// <summary>
	/// Base class of custom columns to be drawn by ProjectWindowDetails
	/// </summary>
    public abstract class ProjectWindowDetailBase
    {
        public bool Visible { get; set; }

        public int ColumnWidth = 100;
        public string Name = "Base";
        public TextAlignment Alignment = TextAlignment.Left;

        public abstract string GetLabel(string guid, string assetPath, Object asset);
    }
}
