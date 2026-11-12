using HilltopApi.Models;

namespace HilltopApi.Data;

public static class SeedData
{
    public static readonly Student[] Students =
    [
        new() { Id = 1,  FirstName = "Tom", LastName = "Max",   School = "Nursing",  DateOfBirth = new DateOnly(1998, 6, 12),  Email = "tmax@hilltop.example",   HomeAddress = "12 Birch St",   Gpa = 3.4 },
        new() { Id = 2,  FirstName = "Ann", LastName = "Fay",   School = "Mining",   DateOfBirth = new DateOnly(2001, 9, 23),  Email = "afay@hilltop.example",   HomeAddress = "8 Quarry Rd",   Gpa = 3.9 },
        new() { Id = 3,  FirstName = "Joe", LastName = "Sun",   School = null,       DateOfBirth = new DateOnly(1995, 4, 17),  Email = "jsun@hilltop.example",   HomeAddress = "41 Elm Ave",    Gpa = 2.8 },
        new() { Id = 4,  FirstName = "Lia", LastName = "Chen",  School = "Medicine", DateOfBirth = new DateOnly(2003, 1, 30),  Email = "lchen@hilltop.example",  HomeAddress = "3 Cedar Ct",    Gpa = 3.7 },
        new() { Id = 5,  FirstName = "Raj", LastName = "Patel", School = "Business", DateOfBirth = new DateOnly(1999, 11, 2),  Email = "rpatel@hilltop.example", HomeAddress = "77 Main St",    Gpa = 3.1 },
        new() { Id = 6,  FirstName = "Ann", LastName = "Smith", School = "Medicine", DateOfBirth = new DateOnly(2000, 7, 8),   Email = "asmith@hilltop.example", HomeAddress = "19 Oak Pl",     Gpa = 3.5 },
        new() { Id = 7,  FirstName = "Kai", LastName = "Lee",   School = "Nursing",  DateOfBirth = new DateOnly(2002, 3, 15),  Email = "klee@hilltop.example",   HomeAddress = "5 Pine Way",    Gpa = 3.2 },
        new() { Id = 8,  FirstName = "Mia", LastName = "Smith", School = "Business", DateOfBirth = new DateOnly(2004, 10, 21), Email = "msmith@hilltop.example", HomeAddress = "19 Oak Pl",     Gpa = 3.8 },
        new() { Id = 9,  FirstName = "Sam", LastName = "Ortiz", School = null,       DateOfBirth = new DateOnly(1997, 12, 5),  Email = "sortiz@hilltop.example", HomeAddress = "220 Hill Rd",   Gpa = 2.9 },
        new() { Id = 10, FirstName = "Eve", LastName = "Nash",  School = "Medicine", DateOfBirth = new DateOnly(2004, 2, 11),  Email = "enash@hilltop.example",  HomeAddress = "64 Maple Dr",   Gpa = 4.0 },
    ];
}
