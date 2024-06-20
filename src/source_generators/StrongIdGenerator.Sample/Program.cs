using Klean.Generated;

var person = new Person(PersonId.NewPersonId());
Console.WriteLine("hello im mr frog, hello");

[StrongId(typeof(Ulid))]
public record Person(PersonId Id);
