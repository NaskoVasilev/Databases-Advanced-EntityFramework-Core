using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using Instagraph.Data;
using Instagraph.Models;
using Instagraph.DataProcessor.Dtos.Import;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.IO;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        private const string PictureImportMessage = "Successfully imported Picture {0}.";
        private const string UserImportMessage = "Successfully imported User {0}.";
        private const string PostImportMessage = "Successfully imported Post {0}.";
        private const string CommentImportMessage = "Successfully imported Comment {0}.";
        private const string UserFollowerImportMessage = "Successfully imported Follower {0} to User {1}.";
        private const string ErrorMessage = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var pictures = JsonConvert.DeserializeObject<Picture[]>(jsonString);
            var uniquePaths = new HashSet<string>();
            var validPictures = new List<Picture>();
            var sb = new StringBuilder();

            foreach (var picture in pictures)
            {
                if (!IsValid(picture) || uniquePaths.Contains(picture.Path))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validPictures.Add(picture);
                sb.AppendLine(string.Format(PictureImportMessage, picture.Path));
            }

            context.Pictures.AddRange(validPictures);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            Dictionary<string, int> profilePictures = context.Pictures.ToDictionary(x => x.Path, y => y.Id);
            var validUsers = new List<User>();
            var sb = new StringBuilder();
            var users = JsonConvert.DeserializeObject<UserImport[]>(jsonString);

            foreach (var user in users)
            {
                if (!IsValid(user) || !profilePictures.ContainsKey(user.ProfilePicture))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User validUser = new User()
                {
                    Username = user.Username,
                    Password = user.Password,
                    ProfilePictureId = profilePictures[user.ProfilePicture]
                };

                validUsers.Add(validUser);
                sb.AppendLine(string.Format(UserImportMessage, user.Username));
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            Dictionary<string, int> users = context.Users.ToDictionary(x => x.Username, y => y.Id);
            var validUsersFollowers = new List<UserFollower>();
            HashSet<ValueTuple<int, int>> userFollowerIds = new HashSet<(int, int)>();
            var sb = new StringBuilder();
            var usersFollowers = JsonConvert.DeserializeObject<UserFollowerImport[]>(jsonString);

            foreach (var userFollower in usersFollowers)
            {
                if (!users.ContainsKey(userFollower.User) || !users.ContainsKey(userFollower.Follower))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                int userId = users[userFollower.User];
                int followerId = users[userFollower.Follower];
                ValueTuple<int, int> tuple = new ValueTuple<int, int>(userId, followerId);

                if (userFollowerIds.Contains(tuple))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                userFollowerIds.Add(tuple);

                UserFollower validUserFollower = new UserFollower()
                {
                    UserId = userId,
                    FollowerId = followerId
                };

                validUsersFollowers.Add(validUserFollower);
                sb.AppendLine(string.Format(UserFollowerImportMessage, userFollower.Follower, userFollower.User));
            }

            context.UsersFollowers.AddRange(validUsersFollowers);
            context.SaveChanges();
            return sb.ToString().ToString().TrimEnd();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            Dictionary<string, int> users = context.Users.ToDictionary(x => x.Username, y => y.Id);
            Dictionary<string, int> pictures = context.Pictures.ToDictionary(x => x.Path, y => y.Id);

            var serializer = new XmlSerializer(typeof(PostImportDto[]), new XmlRootAttribute("posts"));
            var reader = new StringReader(xmlString);
            var posts = (PostImportDto[])serializer.Deserialize(reader);
            var validPosts = new List<Post>();
            var sb = new StringBuilder();

            foreach (var post in posts)
            {
                if (!IsValid(post) || !pictures.ContainsKey(post.Path) || !users.ContainsKey(post.User))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Post validPost = new Post
                {
                    Caption = post.Caption,
                    UserId = users[post.User],
                    PictureId = pictures[post.Path]
                };

                validPosts.Add(validPost);
                sb.AppendLine(string.Format(PostImportMessage, post.Caption));
            }

            context.Posts.AddRange(validPosts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            Dictionary<string, int> users = context.Users.ToDictionary(x => x.Username, y => y.Id);
            HashSet<int> postIds = context.Posts.Select(p => p.Id).ToHashSet();

            var serializer = new XmlSerializer(typeof(CommentImport[]), new XmlRootAttribute("comments"));
            var reader = new StringReader(xmlString);
            var comments = (CommentImport[])serializer.Deserialize(reader);
            var validComments = new List<Comment>();
            var sb = new StringBuilder();

            foreach (var comment in comments)
            {
                if (!IsValid(comment) || !postIds.Contains(comment.Post.Id) || !users.ContainsKey(comment.User))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Comment validComment = new Comment
                {
                    Content = comment.Content,
                    UserId = users[comment.User],
                    PostId = comment.Post.Id
                };

                validComments.Add(validComment);
                sb.AppendLine(string.Format(CommentImportMessage, comment.Content));
            }

            context.Comments.AddRange(validComments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, results, true);
            return isValid;
        }
    }
}