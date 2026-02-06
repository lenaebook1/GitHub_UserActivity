// описывает структуру json-ответа 

using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;

namespace GitHub_UserActivity.Models
{
    class GitHubEvent // представляет одно событие 
    {
        public string? Type { get; set; } // PushEvent, IssuesEvemt, WatchEvent
        public PayLoad? PayLoad { get; set; } // дополнительные данные в зависимости от типа события 
        public Repo? Repo { get; set; } 
        public Actor? Actor { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PayLoad
    {
        public int? Size { get; set; } // количество коммитов 
        public PushCommit[]? Commits { get; set; } // массив коммитов с сообщениями 
        public Issue? Issue { get; set; } 
    }

    public class PushCommit
    {
        public string? Message { get; set; } // 
    }

    public class Issue
    {
        public string? Title { get; set; }
    }

    public class Repo
    {
        public string? Name { get; set; }
    }

    public class Autor
    {
        public string? Login { get; set; }
    }
}
