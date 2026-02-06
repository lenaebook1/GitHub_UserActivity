// взаимодействие с GitHub API

using GitHub_UserActivity.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace GitHub_UserActivity.Services
{
    class GitHubService
    {
        // 1 экземпляр на все приложение. HttpClient - отправка Http-запросов и получение ответов от веб-сервисов 
        public static readonly HttpClient HttpClient = new HttpClient();

        // настройка сопоставления json-файла c нашим классом. передаем в Deserialise параметры 
        // PropertyNameCaseInsensitive = true - при поиске соответствий игнорируй регистр 
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        // 1 кэш на все приложение 
        // readonly гарантирует, что, к примеру, cache нельзя присвоить null
        // ключ - string - имя пользователя. значение - DateTime, List<GitHubEvent> - кортеж (время сохранения данных в кэш, список событий)
        private static readonly Dictionary<string, (DateTime, List<GitHubEvent>)> cache = new();

        // время хранения в кэше 
        private const int CacheMinutes = 5;

        // метод получения событий 
        public async Task<List<GitHubEvent>> GetUserEventsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) {
                throw new ArgumentNullException("username cannot be null or empty.");
            }
            var cacheKey = username.ToLowerInvariant(); // ключ для кэша

            // проверка кэша
            if (cache.TryGetValue(cacheKey, out var cached) && DateTime.UtcNow - cached.Item1 < TimeSpan.FromMinutes(CacheMinutes))
            {
                return cached.Item2;
            }

            // формирование url 
            var url = $"https://api.github.com/users/{username}/events";
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubActivityWpf/1.0");

            // отправка запроса 
            using var response = await HttpClient.GetAsync(url); // get, post, put

            // если статус ответа не 2хх (404)
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Не удалось получить данные: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            List<GitHubEvent> events;
            try
            {
                events = JsonSerializer.Deserialize<List<GitHubEvent>>(json, JsonOptions) ?? [];
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Не удалось разобрать ответ от GitHub.");
            }
            cache[cacheKey] = (DateTime.UtcNow, events.ToList());

            return events.ToList();

        }
        
    }
}
