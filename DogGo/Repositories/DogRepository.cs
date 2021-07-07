using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DogGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
        public class DogRepository : IDogRepository
        {
            private readonly IConfiguration _config;

            // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
            public DogRepository(IConfiguration config)
            {
                _config = config;
            }

            public SqlConnection Connection
            {
                get
                {
                    return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                }
            }
            public List<Dog> GetAllDogs()
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                    ";

                        SqlDataReader reader = cmd.ExecuteReader();

                        List<Dog> dogs = new List<Dog>();
                        while (reader.Read())
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? 
                                        reader.GetString(reader.GetOrdinal("Notes")) : null,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ?
                                        reader.GetString(reader.GetOrdinal("ImageUrl")) : null
                            };

                            dogs.Add(dog);
                        }

                        reader.Close();

                        return dogs;
                    }
                }
            }

            public Dog GetDogById(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                        WHERE Id = @id
                    ";

                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ?
                                        reader.GetString(reader.GetOrdinal("Notes")) : null,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ?
                                        reader.GetString(reader.GetOrdinal("ImageUrl")) : null
                            };

                            reader.Close();
                            return dog;
                        }
                        else
                        {
                            reader.Close();
                            return null;
                        }
                    }
                }
            }

            public Dog GetDogByName(string name)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                        WHERE Name = @name";

                        cmd.Parameters.AddWithValue("@name", name);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            Dog dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ?
                                        reader.GetString(reader.GetOrdinal("Notes")) : null,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ?
                                        reader.GetString(reader.GetOrdinal("ImageUrl")) : null
                            };

                            reader.Close();
                            return dog;
                        }

                        reader.Close();
                        return null;
                    }
                }
            }

            public void AddDog(Dog dog)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                    INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @ownerId, @breed, @notes, @imageUrl);
                ";

                        cmd.Parameters.AddWithValue("@name", dog.Name);
                        cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                        cmd.Parameters.AddWithValue("@breed", dog.Breed);
                        if (dog.Notes == null)
                        {
                            cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@notes", dog.Notes);
                        };
                        if (dog.ImageUrl == null)
                        {
                            cmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ImageUrl", dog.ImageUrl);
                        };

                        int id = (int)cmd.ExecuteScalar();

                        dog.Id = id;
                    }
                }
            }

            public void UpdateDog(Dog dog)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Dog
                            SET 
                                [Name] = @name, 
                                OwnerId = @ownerId, 
                                Breed = @breed, 
                                Notes = @notes, 
                                ImageUrl = @imageUrl
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    if (dog.Notes == null)
                    {
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    };
                    if (dog.ImageUrl == null)
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", dog.ImageUrl);
                    };

                    cmd.ExecuteNonQuery();
                    }
                }
            }

            public void DeleteDog(int dogId)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM Dog
                            WHERE Id = @id
                        ";

                        cmd.Parameters.AddWithValue("@id", dogId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
}
