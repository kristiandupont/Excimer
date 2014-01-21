using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excimer;

namespace TestHelpers
{
    public class TestViewModel
    {
        public static TestViewModel Instance;

        public TestViewModel()
        {
            Console.WriteLine("Constructing TestViewModel");
            Instance = this;
        }

        public Observable<string> StringProperty = new Observable<string>();
    }
}
