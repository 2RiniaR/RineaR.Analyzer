using System.Collections.Generic;
using RineaR.Analyzer;

namespace RineaR.Analyzer.Sample;

public class Sample
{
    public static void SampleMethod1(Spaceship spaceship)
    {
        var list = new List<Spaceship>();

        list.FirstOr(x => x == spaceship, t => spaceship.SetSpeed(42), () => spaceship.SetSpeed(0));
    }
    
    public class Spaceship
    {
        public void SetSpeed(int speed)
        {
            // Do something with speed
        }
    }
}