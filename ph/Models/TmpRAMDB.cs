using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ph.Models
{
    public static class TmpRAMDB
    {
        private static ObservableCollection<User> _users;
        private static ObservableCollection<Pet> _pets;
        private static ObservableCollection<Post> _posts;

        private static void Flush(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_users != null)
            {
                File.WriteAllText(
                    "./tmpdb/users.json",
                    JsonConvert.SerializeObject(_users)
                );
            }

            if (_pets != null)
            {
                File.WriteAllText(
                    "./tmpdb/pets.json",
                    JsonConvert.SerializeObject(_pets)
                );
            }

            if (_posts != null)
            {
                File.WriteAllText(
                    "./tmpdb/posts.json",
                    JsonConvert.SerializeObject(_posts)
                );
                
            }
        }
        
        public static ICollection<User> Users()
        {
            if (_users == null)
            {
                _users = JsonConvert.DeserializeObject<ObservableCollection<User>>(File.ReadAllText("./tmpdb/users.json"));
                _users.CollectionChanged += Flush;
            }
            return _users;
        }

        public static ICollection<Pet> Pets()
        {
            if (_pets == null)
            {
                _pets = JsonConvert.DeserializeObject<ObservableCollection<Pet>>(File.ReadAllText("./tmpdb/pets.json"));
                _pets.CollectionChanged += Flush;
            }
            return _pets;
        }

        public static ICollection<Post> Posts()
        {
            if (_posts == null)
            {
                _posts = JsonConvert.DeserializeObject<ObservableCollection<Post>>(
                    File.ReadAllText("./tmpdb/posts.json"));
                _posts.CollectionChanged += Flush;
            }
            return _posts;
        }
    }
}