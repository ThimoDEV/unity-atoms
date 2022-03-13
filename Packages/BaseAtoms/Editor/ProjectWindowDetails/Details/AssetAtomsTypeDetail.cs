///MIT License
///Copyright(c) 2019 InnoGames GmbH
///
using UnityEngine;

namespace UnityAtoms.Editor
{
    public class AssetAtomsTypeDetail : ProjectWindowDetailBase
    {
        public AssetAtomsTypeDetail()
        {
            Name = "Asset Type";
            ColumnWidth = 100;
        }
        public override string GetLabel(string guid, string assetPath, Object asset)
        {
            if(asset is BaseAtom)
            {
                return asset.GetType().Name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

