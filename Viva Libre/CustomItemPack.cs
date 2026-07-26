using CustomItems;
using HawkNetworking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viva_Libre
{
    public class CustomItemPack
    {
        public string path;
        public string packName;
        public string packAuthor;
        public List<CustomItem> items = new();
        public List<AssetBundle> assetBundles = new();
        public List<Assembly> assemblies = new();

        public CustomItemPack(string path)
        {
            this.path = path;

            var jsonPath = Path.Combine(path, "data.json");
            var json = File.ReadAllText(jsonPath);
            var data = JsonConvert.DeserializeObject<JsonData>(json);

            packName = data.name;
            packAuthor = data.author;

            if (data.assemblyPath != null && data.assemblyPath != "")
            {
                var assemblyPath = Path.Combine(path,data.assemblyPath);
                assemblies.Add(Assembly.LoadFile(Path.GetFullPath(assemblyPath)));
            }

            if (data.assemblyPaths != null)
            {
                foreach (var assemblyPath in data.assemblyPaths)
                {
                    var fullPath = Path.Combine(path, assemblyPath);
                    assemblies.Add(Assembly.LoadFile(fullPath));
                }
            }


            if (data.assetPath != null && data.assetPath != "")
            {
                var assetPath = Path.Combine(path, data.assetPath);
                var bundle = AssetBundle.LoadFromFile(assetPath);
                assetBundles.Add(bundle);

                foreach (var obj in bundle.LoadAllAssets<GameObject>())
                {
                    if (obj.TryGetComponent<CustomItem>(out var item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (data.assetPaths != null)
            {
                foreach (var assetPath in data.assetPaths)
                {
                    var fullPath = Path.Combine(path,assetPath);
                    var bundle = AssetBundle.LoadFromFile(fullPath);
                    assetBundles.Add(bundle);

                    foreach (var obj in bundle.LoadAllAssets<GameObject>())
                    {
                        if (obj.TryGetComponent<CustomItem>(out var item))
                        {
                            items.Add(item);
                        }
                    }
                }
            }


            foreach (var item in items)
            {
                HawkNetworkManager.DefaultInstance.RegisterPrefab(item);
            }
        }

        public class JsonData
        {
            public string name;
            public string author;
            public string assetPath;
            public string[] assetPaths;
            public string assemblyPath;
            public string[] assemblyPaths;
        }
    }
}
