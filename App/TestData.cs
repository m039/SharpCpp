using System;
namespace SharpCpp
{
    public static class TestData
    {

        public const string InputCode = InputCode3;

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
        
    }
}
