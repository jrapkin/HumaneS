using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }
            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    CreateEmployee(employee);
                    break;
                case "read":
                    Employee fetchedEmployee = FetchEmployeeInfo(employee);
                    UserInterface.DisplayEmployeeInfo(fetchedEmployee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "delete":
                    DeleteEmployee(employee);
                    break;
            }
        }

        internal static void CreateEmployee(Employee employeeToAdd)
        {
            db.Employees.InsertOnSubmit(employeeToAdd);
            db.SubmitChanges();
        }
        internal static Employee FetchEmployeeInfo(Employee employee)
        {
            employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
            return employee;
        }
        internal static void UpdateEmployee(Employee employeeWithUpdates)
        {
            Employee employeeFromDb = null;

            try
            {
                employeeFromDb = db.Employees.Where(e => e.EmployeeId == employeeWithUpdates.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            employeeFromDb.FirstName = employeeWithUpdates.FirstName;
            employeeFromDb.LastName = employeeWithUpdates.LastName;
            employeeFromDb.UserName = employeeWithUpdates.UserName;
            employeeFromDb.Password = employeeWithUpdates.Password;
            employeeFromDb.Email = employeeWithUpdates.Email;

            db.SubmitChanges();
        }
        internal static void DeleteEmployee(Employee employee)
        {
            try
            {
                Employee employeeToDelete = new Employee();
                employeeToDelete = db.Employees.Where(emp => emp.LastName == employee.LastName && emp.EmployeeNumber == employee.EmployeeNumber).Single();
                db.Employees.DeleteOnSubmit(employeeToDelete);
                db.SubmitChanges();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No Employees have an EmployeeNumber that matches the Employee passed in.");
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animal = db.Animals.Where(animalid => animalid.AnimalId == id).Single();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalToUpdate = null;
            try
            {
                animalToUpdate = db.Animals.Where(animal => animal.AnimalId == animalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No animals with a matching animal ID were passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            foreach (KeyValuePair<int,string> entry in updates)
            {
                switch (entry.Key)
                {

                    case 1:
                        animalToUpdate.CategoryId = db.Categories.Where(category => category.Name == entry.Value).Select(categoryNumber => categoryNumber.CategoryId).Single();
                        break;
                    case 2:
                        animalToUpdate.Name = entry.Value;
                        break;
                    case 3:
                        animalToUpdate.Age = Convert.ToInt32(entry.Value);
                        break;
                    case 4:
                        animalToUpdate.Demeanor = entry.Value;
                        break;
                    case 5:
                        animalToUpdate.KidFriendly = Convert.ToBoolean(entry.Value);
                        break;
                    case 6:
                        animalToUpdate.PetFriendly = Convert.ToBoolean(entry.Value);
                        break;
                    case 7:
                        animalToUpdate.Weight = Convert.ToInt32(entry.Value);
                        break;
                }
            }
            db.SubmitChanges();
        }


        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> traits) // parameter(s)?
        {
            IQueryable<Animal> animalList = null;

            foreach (KeyValuePair<int, string> entry in traits)
            {
                switch (entry.Key)
                {

                    case 1:
                        int categoryID = GetCategoryId(entry.Value);
                        animalList = db.Animals.Where(category => category.CategoryId == categoryID);
                        break;
                    case 2:
                        animalList = db.Animals.Where(a => a.Name == entry.Value);
                        break;
                    case 3:
                        animalList = db.Animals.Where(a => a.Age == Convert.ToInt32(entry.Value));
                        break;
                    case 4:
                        animalList = db.Animals.Where(a => a.Demeanor == entry.Value);
                        break;
                    case 5:
                        animalList = db.Animals.Where(a => a.KidFriendly == Convert.ToBoolean(entry.Value));
                        break;
                    case 6:
                        animalList = db.Animals.Where(a => a.PetFriendly == Convert.ToBoolean(entry.Value));
                        break;
                    case 7:
                        animalList = db.Animals.Where(a => a.Weight == Convert.ToInt32(entry.Value));
                        break;
                    case 8:
                        animalList = db.Animals.Where(a => a.AnimalId == Convert.ToInt32(entry.Value));
                        break;
                }
            }
            return animalList;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryId = 0;
            try
            {
                categoryId = db.Categories.Where(category => category.Name == categoryName).Select(id => id.CategoryId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No category with a matching category name were passed in.");
                Console.WriteLine("No update have been made.");
            }
            return categoryId;
        }

        internal static Room GetRoom(int animalId)
        {
            Room room = null;
            try
            {
                room = db.Rooms.Where(roomWithAnimal => roomWithAnimal.AnimalId == animalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No rooms with a matching animal ID were passed in.");
                Console.WriteLine("No update have been made.");
            }
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = 0;
            try
            {
                dietPlanId = db.DietPlans.Where(plan => plan.Name == dietPlanName).Select(id => id.DietPlanId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No diet plans with a matching diet plan name were passed in.");
                Console.WriteLine("No update have been made.");
            }
            return dietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoptionRequest = new Adoption();
            adoptionRequest.ClientId = client.ClientId;
            adoptionRequest.AnimalId = animal.AnimalId;
            adoptionRequest.PaymentCollected = false;
            adoptionRequest.ApprovalStatus = "Pending";

            db.Adoptions.InsertOnSubmit(adoptionRequest);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var listOfPendingAdoptions = db.Adoptions.Where(adoption => adoption.ApprovalStatus == "Pending");
            return listOfPendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            Adoption adoptionToUpdate = db.Adoptions.Where(application => application.AnimalId == adoption.AnimalId && application.ClientId == adoption.ClientId).FirstOrDefault();
            switch (isAdopted)
            {
                case true:
                    adoptionToUpdate.ApprovalStatus = "Approved";
                    adoptionToUpdate.PaymentCollected = true;
                    db.SubmitChanges();
                    break;
                case false:
                    adoptionToUpdate.ApprovalStatus = "Denied";
                    db.SubmitChanges();
                    break;
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoptionToRemove = db.Adoptions.Where(application => application.AnimalId == animalId && application.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptionToRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot shotGiven = db.Shots.Where(shot => shot.Name == shotName).FirstOrDefault();
            AnimalShot shotToUpdate = db.AnimalShots.Where(animalShot => animalShot.AnimalId == animal.AnimalId).Where(shot => shot.ShotId==shotGiven.ShotId).SingleOrDefault();
            shotToUpdate.DateReceived = System.DateTime.Today;
            db.SubmitChanges();
        }
    }
}