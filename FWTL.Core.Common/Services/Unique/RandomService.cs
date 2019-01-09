using System;

namespace FWTL.Core.Services.Unique
{
    public class RandomService : IRandomService
    {
        public Random Random { get; } = new Random();
    }
}
