// Core/Interfaces/Services/IAiConnectionTester.cs (YENİ)
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using YetenekPusulasi.Core.Interfaces.AI;

namespace YetenekPusulasi.Core.Interfaces.Services
{
    public interface IAiConnectionTester
    {
        Task<(bool isSuccess, string message)> TestConnectionAsync(string aiModelIdentifier);
    }
}
