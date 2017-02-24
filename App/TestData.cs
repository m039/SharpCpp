using System;
namespace SharpCpp
{
    public static class TestData
    {

        public const string InputCode = InputCode2;

        // done
        private const string InputCode1 = @"namespace CSharpFooBarLibrary 
{
    public class Foo {

        public int number = 0;

    }
}";

        private const string InputCode2 = @"namespace CSharpFooBarLibrary
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
        
    }
}
