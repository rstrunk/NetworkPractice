using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AudioGameLearning
{
    public class SoundLibrary
    {
        private Dictionary<string, SoundEffect> _sounds = new();
        private ContentManager _content;
        public SoundLibrary(ContentManager content)
        {
            _content = content;
        }

        public void Load(string key, string contentPath)
        {
_sounds[key] = _content.Load<SoundEffect>(contentPath);
        }

        public void LoadFromManifest(string filePath)
        {
            string json = File.ReadAllText(filePath);
                        List<SoundManifestEntry>? entries = JsonSerializer.Deserialize<List<SoundManifestEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
                if (entries == null)
    {
        System.Console.WriteLine("SoundLibrary: failed to deserialize sounds.json.");
        return;
    }
                        foreach(SoundManifestEntry entry in entries)
                        {
                 if(entry.Key != null && entry.Path != null)
                 Load(entry.Key, entry.Path);
                        }
                    }

        public SoundEffect? Get(string key)
        {
            if(_sounds.TryGetValue(key, out SoundEffect? sound)) return sound;
                        System.Console.WriteLine($"SoundLibrary: key '{key}' not found.");
            return null;
        }

                public bool TryGet(string key, out SoundEffect? sound)
        {
            return _sounds.TryGetValue(key, out sound);
        }
    }
}