﻿using FPTUMerchAPI.Model;
using FireSharp.Extensions;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPTUMerchAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + @"fptumerch.json";
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Users> usersList = new List<Users>();
                Query Qref = database.Collection("Users").OrderBy("RoleID");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();

                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Users user = docsnap.ConvertTo<Users>();
                        user.UserID = docsnap.Id;
                        usersList.Add(user);
                    }
                }
                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByUserId(string id)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Users> usersList = new List<Users>();
                Query Qref = database.Collection("Users");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();

                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Users user = docsnap.ConvertTo<Users>();
                        user.UserID = docsnap.Id;
                        usersList.Add(user);
                    }
                }
                if (id == null || id.Equals("") || id.Length == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(usersList.Where(x => x.UserID.Equals(id)));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByRoleId(string id)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Users> usersList = new List<Users>();
                Query Qref = database.Collection("Users");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();

                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Users user = docsnap.ConvertTo<Users>();
                        user.UserID = docsnap.Id;
                        usersList.Add(user);
                    }
                }
                if (id == null || id.Equals("") || id.Length == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(usersList.Where(x => x.RoleID.Equals(id)));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetByEmailAndPassword([FromBody] UserAuthentication userCheck)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                CollectionReference coll = database.Collection("Users");
                List<Users> usersList = new List<Users>();
                Query Qref = database.Collection("Users");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();

                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Users user = docsnap.ConvertTo<Users>();
                        user.UserID = docsnap.Id;
                        if (userCheck.Email == user.Email
                            && userCheck.Password == user.Password)
                            //Check ignore case
                        {
                            return Ok(user.ToJson());
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Users user)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                Query qRefUser = database.Collection("Users");
                QuerySnapshot qSnapUser = await qRefUser.GetSnapshotAsync();
                bool roleCheck = true; //Check if the role is correct
                bool ableToCreate = true; // Check if the account can be create 
                string roleName = "";
                foreach(DocumentSnapshot docSnapUser in qSnapUser)
                {
                    if (docSnapUser.Exists)
                    { // If there is an account/some accounts in the database
                        //1) Check if email already exist in the database
                        Users userCheck = docSnapUser.ConvertTo<Users>();
                        userCheck.UserID = docSnapUser.Id;
                        if (userCheck.Email == user.Email)
                        {// 1.2) If the new user email already exist
                            return Conflict("The user email already exist");
                        }
                        else
                        {// 1.1) If the new user email not exist yet
                            Query qRefRole = database.Collection("Role");
                            QuerySnapshot qSnapRole = await qRefRole.GetSnapshotAsync();
                            foreach (DocumentSnapshot docSnapRole in qSnapRole)
                            {
                                Role role = docSnapRole.ConvertTo<Role>();
                                role.RoleID = docSnapRole.Id;
                                if(role.RoleID == user.RoleID)
                                { // 1.1.1)If the role is correct
                                    roleCheck = true;
                                    roleName = role.RoleName;
                                    break;
                                }
                                else
                                { // 1.1.2) If the role is incorrect
                                    roleCheck = false;
                                    continue;
                                }
                            }
                            if (!roleCheck)
                            {
                                return BadRequest("RoleID not exist");
                            }
                            else
                            {
                                DocumentReference docRefDiscountCode = database.Collection("DiscountCode").Document(user.DiscountCodeID);
                                DocumentSnapshot docSnapDiscountCode = await docRefDiscountCode.GetSnapshotAsync();
                                if (roleName == "Saler")
                                { // If the new user has the role "Saler"
                                    if (!docSnapDiscountCode.Exists)
                                    {// If the discount code of the new user isn't exist
                                        return BadRequest("DiscountID not exist");
                                    }
                                    else
                                    {
                                        //1.1.1.1) Check if the discountCodeID already have the user assign to it
                                        if (userCheck.DiscountCodeID == user.DiscountCodeID)
                                        {
                                            return BadRequest("DiscountID already assign to a user, please try again");
                                        }
                                        else
                                        {
                                            ableToCreate = true;
                                            continue;
                                        }
                                    }
                                } 
                                else if(roleName == "Admin" || roleName == "Logistic")
                                {// If the new user has the role "Admin"
                                    ableToCreate = true;
                                    continue;
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                        }
                    }
                }
                if(qSnapUser.Count == 0)
                { // If there is 0 account in the database
                        Query qRefRole = database.Collection("Role");
                        QuerySnapshot qSnapRole = await qRefRole.GetSnapshotAsync();
                        foreach (DocumentSnapshot docSnapRole in qSnapRole)
                        {
                            Role role = docSnapRole.ConvertTo<Role>();
                            role.RoleID = docSnapRole.Id;
                            if (role.RoleID == user.RoleID)
                            { // 1.1.1)If the role is correct
                                roleCheck = true;
                                roleName = role.RoleName;
                                break;
                            }
                            else
                            { // 1.1.2) If the role is incorrect
                                roleCheck = false;
                                continue;
                            }
                        }
                        if (!roleCheck)
                        {
                            return BadRequest("RoleID not exist");
                        }
                        else
                        {
                            if (user.DiscountCodeID == null || user.DiscountCodeID.Length ==0 || user.DiscountCodeID == "")
                            {
                                return BadRequest("DiscountID cannot be null");
                            }
                            else
                            {
                                DocumentReference docRefDiscountCode = database.Collection("DiscountCode").Document(user.DiscountCodeID);
                                DocumentSnapshot docSnapDiscountCode = await docRefDiscountCode.GetSnapshotAsync();
                                if(roleName == "Saler")
                                {
                                    if (!docSnapDiscountCode.Exists)
                                    {// If the discount code of the new user isn't exist
                                        return BadRequest("DiscountID not exist");
                                    }
                                    else
                                    {
                                        ableToCreate = true;
                                    }
                                }
                                else
                                {
                                    ableToCreate = true;
                                }
                            }
                        }
                }
                if (ableToCreate)
                {
                    CollectionReference collRefUser = database.Collection("Users");
                    Dictionary<string, object> newUser = new Dictionary<string, object>() {
                        {"FullName", user.FullName },
                        {"Email", user.Email },
                        {"Password", user.Password },
                        {"Note", user.Note },
                        {"RoleID", user.RoleID }
                    };
                    if(roleName == "Admin")
                    {// If the new user has the role "Admin"
                        newUser.Add("DiscountCodeID", null);
                    } else if(roleName == "Saler")
                    {// If the new user has the role "Saler"
                        newUser.Add("DiscountCodeID", user.DiscountCodeID);
                    }
                    await collRefUser.AddAsync(newUser);
                    return Ok(newUser.ToJson());
                }
                else
                {
                    return BadRequest("Something wrong, please try again");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<UsersController>/5
        [HttpPut("{userId}")]
        public async Task<ActionResult> Put(string userId, [FromBody] Users user)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRefUser = database.Collection("Users").Document(userId);
                DocumentSnapshot docSnapUser = await docRefUser.GetSnapshotAsync();
                if (docSnapUser.Exists)
                { // If user ID Exist, only update fullName, email, password, note
                    Users currentUser = docSnapUser.ConvertTo<Users>();
                    Dictionary<string, object> updateUser = new Dictionary<string, object>()
                    {
                        {"FullName", user.FullName },
                        {"Email", user.Email },
                        {"Password", user.Password },
                        {"Note", user.Note },
                        {"RoleID", currentUser.RoleID }
                    };
                    //Check if new email already exist:
                    Query qRefUsers = database.Collection("Users");
                    QuerySnapshot qSnapUsers = await qRefUsers.GetSnapshotAsync();
                    foreach(DocumentSnapshot docSnapUserCheck in qSnapUsers)
                    {
                        Users userCheck = docSnapUserCheck.ConvertTo<Users>();
                        if(userCheck.Email == user.Email)
                        { // if new email already exist
                            return BadRequest("The new email is currently exist");
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //----------------------------------------------------
                    await docRefUser.SetAsync(updateUser);
                    return Ok(updateUser.ToJson());
                }
                else
                { // If user ID not exist
                    return NotFound("User ID not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Users").Document(id);
                DocumentSnapshot snap = await docRef.GetSnapshotAsync();
                if (snap.Exists)
                {
                    await docRef.DeleteAsync();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
