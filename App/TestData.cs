using System;
namespace SharpCpp
{
    public static class TestData
    {

        public const string InputCode = InputCode4;

        // done
        const string InputCode1 = @"namespace CSharpFooBarLibrary 
{
    public class Foo {

        public int number = 0;

    }
}";

        // done
        const string InputCode2 = @"namespace CSharpFooBarLibrary
{
    public class Foo {

        public int number = 0;

        public int GetNumber() {
            return number;
        }

        public void SetNumber(int number) {
            this.number = number;
        }

    }
}";

        // done
        const string InputCode3 = @"namespace CSharpFooBarLibrary
{
    public interface IBar
    {
        int GetNumber();
    }

    public class Foo {

        public int number = 0;

        public int GetNumber() {
            return number;
        }

        public void SetNumber(int number) {
            this.number = number;
        }

    }
}";

        const string InputCode4 = @"namespace CSharpFooBarLibrary
{
    public class Bar
    {
        private IBar impl;

        public void Register(IBar ibar)
        {
            this.impl = ibar;
        }

        public int PerformGetNumber()
        {
            if (impl == null)
            {
                return -1;
            }
            else
            {
                return impl.GetNumber();
            }
        }
    }

    public interface IBar
    {
        int GetNumber();
    }

    public class Foo {

        public int number = 0;

        public int GetNumber() {
            return number;
        }

        public void SetNumber(int number) {
            this.number = number;
        }

    }
}";
        
    }
}
