using Couresework2.CoinCollectionApp.Models;
using System.Text.Json;

namespace CoinCollectionApp.Services
{
    public class DataService
    {
        private readonly string _baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoinCollectionData");
        private readonly string _imagesDir;
        private readonly string _collectionFile;
        private readonly string _dictsFile;

        public DataService()
        {
            _imagesDir = Path.Combine(_baseDir, "Images");
            _collectionFile = Path.Combine(_baseDir, "collection.json");
            _dictsFile = Path.Combine(_baseDir, "dictionaries.json");

            if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
            if (!Directory.Exists(_imagesDir)) Directory.CreateDirectory(_imagesDir);
        }

        public CollectionData LoadCollection()
        {
            if (!File.Exists(_collectionFile)) return new CollectionData();

            try
            {
                string json = File.ReadAllText(_collectionFile);
                return JsonSerializer.Deserialize<CollectionData>(json) ?? new CollectionData();
            }
            catch
            {
                return new CollectionData();
            }
        }

        public void SaveCollection(CollectionData data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_collectionFile, json);
        }

        public DictionaryData LoadDictionaries()
        {
            if (!File.Exists(_dictsFile)) return CreateDefaultDictionaries();

            try
            {
                string json = File.ReadAllText(_dictsFile);
                return JsonSerializer.Deserialize<DictionaryData>(json) ?? CreateDefaultDictionaries();
            }
            catch
            {
                return CreateDefaultDictionaries();
            }
        }

        public void SaveDictionaries(DictionaryData data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_dictsFile, json);
        }

        private DictionaryData CreateDefaultDictionaries()
        {
            var defaultData = new DictionaryData();

            defaultData.Countries.Add(new Country { Name = "СССР" });
            defaultData.Countries.Add(new Country { Name = "Российская Империя" });
            defaultData.Countries.Add(new Country { Name = "Российская Федерация" });
            defaultData.Countries.Add(new Country { Name = "США" });

            defaultData.Metals.Add(new Metal { Name = "Серебро" });
            defaultData.Metals.Add(new Metal { Name = "Золото" });
            defaultData.Metals.Add(new Metal { Name = "Медь" });
            defaultData.Metals.Add(new Metal { Name = "Медно-никелевый сплав" });

            defaultData.Conditions.Add(new Condition { Name = "UNC", Description = "Превосходное (Штемпельный блеск, нет следов обращения)" });
            defaultData.Conditions.Add(new Condition { Name = "XF", Description = "Отличное (Минимальный износ, четкий рельеф)" });
            defaultData.Conditions.Add(new Condition { Name = "VF", Description = "Очень хорошее (Заметный износ, но все детали видны)" });

            SaveDictionaries(defaultData);
            return defaultData;
        }

        public string CopyImageToStorage(string sourceFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
                return null;

            try
            {
                string extension = Path.GetExtension(sourceFilePath);

                string newFileName = $"{Guid.NewGuid()}{extension}";
                string destPath = Path.Combine(_imagesDir, newFileName);

                File.Copy(sourceFilePath, destPath, true);

                return Path.Combine("Images", newFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка копирования фото: {ex.Message}");
                return null;
            }
        }

        public string GetAbsoluteImagePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            return Path.Combine(_baseDir, relativePath);
        }

        public void DeleteCoinImages(List<string> relativePaths)
        {
            if (relativePaths == null || relativePaths.Count == 0) return;

            foreach (var path in relativePaths)
            {
                if (string.IsNullOrEmpty(path)) continue;

                string absolutePath = GetAbsoluteImagePath(path);
                try
                {
                    if (File.Exists(absolutePath))
                    {
                        File.Delete(absolutePath);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Не удалось удалить файл {path}: {ex.Message}");
                }
            }
        }

        public void DeleteSingleImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string absolutePath = GetAbsoluteImagePath(relativePath);
            try
            {
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Не удалось удалить файл {relativePath}: {ex.Message}");
            }
        }
    }
}
