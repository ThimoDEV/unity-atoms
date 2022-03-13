///MIT License
///Copyright(c) 2019 InnoGames GmbH
///
using UnityEngine;

namespace UnityAtoms.Editor
{
    public class GuidDetail : ProjectWindowDetailBase
    {
        public GuidDetail()
        {
            Name = "Guid";
            ColumnWidth = 240;
        }
        public override string GetLabel(string guid, string assetPath, Object asset)
        {
            return guid;
        }
    }
}


