using System;

namespace IocDemo
{
    public interface IMain
    {
        void Run();
    }

    public interface IThreadA
    {
        void Start();
    }

    public interface IThreadB
    {
        void Start();
    }

    public interface IThreadC
    {
        void Start();
    }

    public class ThreadA : IThreadA
    {
        public void Start()
        {
            Console.WriteLine("ThreadA is running.");
        }
    }


    public class ThreadB : IThreadB
    {
        public ThreadB(IThreadC threadC1, IThreadC threadC2)
        {
            if (threadC1 == threadC2)
            {
                Console.WriteLine("threadC1 == threadC2");
            }
        }

        public void Start()
        {
            Console.WriteLine("ThreadB is running.");
        }
    }

    public class ThreadC : IThreadC
    {
        public void Start()
        {
            Console.WriteLine("ThreadC is running.");
        }
    }

    public class Main : IMain
    {
        private IThreadA _threadA;
        private IThreadB _threadB;

        public Main(IThreadA threadA, IThreadB threadB)
        {
            _threadA = threadA;
            _threadB = threadB;
        }

        public void Run()
        {
            _threadA.Start();
            _threadB.Start();
        }
    }
}