
                        SELECT Walker.Id, Walker.[Name], Walker.ImageUrl, Walker.NeighborhoodId, Neighborhood.Name as Neighborhood
                        FROM Walker
                        Left Join Neighborhood on Neighborhood.Id = Walker.NeighborhoodId
                    