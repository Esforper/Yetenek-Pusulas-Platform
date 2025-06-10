using System.Collections.Generic;
using YetenekPusulasi.Core.Entities;
using YetenekPusulasi.Core.Interfaces.Entities; // IScenario için
//using Microsoft.AspNetCore.Html; // IHtmlContent için (Strategy kullanırsanız)

namespace YetenekPusulasi.WebApp.Models.StudentViewModels
{
    public class ViewScenarioViewModel
    {
        public IScenario Scenario { get; set; }
        public List<StudentAnswer> StudentAnswers { get; set; } // Bu senaryoya ait öğrenci cevapları
        // public IHtmlContent DisplayHtml { get; set; } // Strategy'den gelen HTML
    }
}