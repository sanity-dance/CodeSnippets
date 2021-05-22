using System;
using System.Collections.Generic;

namespace ExtensibleZoo
{
    /*
    
    In the below pattern, we are implementing a central controller class (Zoo) that has a list of Animal.

    Animal is an abstract class from which several subclasses (like Tiger and Giraffe) inherit from.

    Zoo knows about certain animals, which are stored in the _supportedAnimals dictionary as a string and a function that build the animal in question.

    A publicly exposed AddSupportedAnimal method allows clients to teach the Zoo about new animals they have defined, which the Zoo can then add to the
        OurAnimals collection.

    This is useful when you are processing a Json that gives you a Type token, for example, that indicates which type of animal the rest of the properties
        in the Json should apply to.
    
    This approach provides two benefits. First, we avoid an inconvenient switch case in Zoo that has to grow every time we add a new Animal. If we are
        validating input before allowing Zoo to attempt to build its list of animals or displaying the animals the Zoo knows about in a help function,
        we would need to keep track of supported animals somewhere outside the switch case, which is another place that has to be updated when you want
        to add a new Animal.

    Secondly, a client for our Zoo library is able to easily implement their own Animal subclasses and teach the Zoo about them.

    */
    
    public class Zoo
    {
        string Name { get; set; }
        public List<Animal> OurAnimals { get; set; } = new List<Animal>();

        static Dictionary<string, Func<Animal>> _supportedAnimals = new Dictionary<string,Func<Animal>>() { { "Giraffe", MakeAnimal<Giraffe>() }, { "Tiger", MakeAnimal<Tiger>() } };

        public static void AddSupportedAnimal(string animalName, Func<Animal> AnimalMaker)
        {
            _supportedAnimals.Add(animalName, AnimalMaker);
        }

        // This function is necessary to preserve the subtypes being added to OurAnimals. This also means that all Animals need a static factory method
        //      for instantiation.

        public static Func<T> MakeAnimal<T>() where T : new()
        {
            static T internalMakeAnimal()
            {
                return new T();
            }
            return internalMakeAnimal;
        }

        public Zoo(List<string> animalList)
        {
            // This is where we instantiate fresh Animals with the delegate created from MakeAnimal. After the fresh instance is created, the static
            //      factory method of that class can be called. In the final OurAnimals list, the type of the animal is preserved.
            foreach(var animal in animalList)
            {
                if(_supportedAnimals.ContainsKey(animal))
                {
                    Animal newAnimal = _supportedAnimals[animal]();
                    newAnimal.BuildAnimal(new List<string>());
                    OurAnimals.Add(newAnimal);
                }
                else
                {
                    Console.WriteLine("Unsupported animal type: " + animal);
                }
            }
        }
    }

    public abstract class Animal
    {
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public abstract void BuildAnimal(List<string> myArgs);
    }

    public class Giraffe : Animal
    {
       public override void BuildAnimal(List<string> myArgs)
       {
            Attributes.Add("Necc","long");
            Attributes.Add("Honger", "leaves");
       }
    }

    public class Tiger : Animal
    {
        public override void BuildAnimal(List<string> myArgs)
        {
            Attributes.Add("Claws", "sharp");
            Attributes.Add("Honger", "manflesh");
            Attributes.Add("Sound", "meow");
        }
    }

    // Sample client code. The "Snehk" class is created by the client and the Zoo is taught about it by the client.

    public class Snehk : Animal
    {
        public override void BuildAnimal(List<string> myArgs)
        {
            Attributes.Add("Legs", "no");
            Attributes.Add("Honger", "mouses");
            Attributes.Add("Scales", "shiny");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Zoo.AddSupportedAnimal("Snake", Zoo.MakeAnimal<Snehk>());
            Zoo myZoo = new Zoo(new List<string>() { "Giraffe", "Tiger", "Tiger", "Snake" } );
        }
    }
}
