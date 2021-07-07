﻿using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        List<Dog> GetAllDogs();

        Dog GetDogById(int id);

        Dog GetDogByName(string name);

        void AddDog(Dog dog);

        void UpdateDog(Dog dog);

        void DeleteDog(int dogId);
    }
}