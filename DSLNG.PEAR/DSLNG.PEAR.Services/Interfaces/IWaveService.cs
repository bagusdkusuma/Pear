using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Wave;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Wave;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IWaveService
    {
        GetWavesResponse GetWaves(GetWavesRequest request);
        GetWavesResponse GetWavesForGrid(GetWavesRequest request);
        SaveWaveResponse SaveWave(SaveWaveRequest request);
        GetWaveResponse GetWave(GetWaveRequest request);
    }
}
