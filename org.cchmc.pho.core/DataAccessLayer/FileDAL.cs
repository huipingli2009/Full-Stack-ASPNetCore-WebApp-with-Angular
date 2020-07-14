using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace org.cchmc.pho.core.DataAccessLayer
{
    public class FileDAL : IFiles
    {
        private readonly ConnectionStrings _connectionStrings; 
        private readonly ILogger<FileDAL> _logger;

        public FileDAL(IOptions<ConnectionStrings> options, ILogger<FileDAL> logger)
        {
            _connectionStrings = options.Value;
            _logger = logger;
        }


        public async Task<List<File>> ListFiles(int userId, int? resourceTypeId, int? initiativeId, string tag, bool? watch, string name)
        {
            DataTable dataTable = new DataTable();
            List<File> file = new List<File>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFiles", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ResourceTypeId", SqlDbType.Int).Value = resourceTypeId;
                    sqlCommand.Parameters.Add("@InitiativeId", SqlDbType.Int).Value = initiativeId;
                    sqlCommand.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag;
                    sqlCommand.Parameters.Add("@Watch", SqlDbType.Bit).Value = watch;
                    sqlCommand.Parameters.Add("@NameSearch", SqlDbType.VarChar).Value = name;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        List<FileTag> fileTags = await GetFileTagsAll();
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new File()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                DateCreated = (dr["DateCreated"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DateCreated"].ToString())),
                                LastViewed = (dr["LastViewed"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastViewed"].ToString())),
                                WatchFlag = (dr["WatchFlag"] != DBNull.Value && Convert.ToBoolean(dr["WatchFlag"])),
                                FileURL = dr["FileURL"].ToString(),
                                PublishFlag = (dr["Published"] != DBNull.Value && Convert.ToBoolean(dr["Published"])),
                                Description = dr["Description"].ToString(),
                                Tags = new List<FileTag>()
                            };
                            if (!string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
                            {
                                foreach (string tagName in dr["Tags"].ToString().Split(',').Select(t => Convert.ToString(t)))
                                {
                                    if (fileTags.Any(f => f.Name == tagName))
                                        record.Tags.Add(fileTags.First(f => f.Name == tagName));
                                    else
                                        _logger.LogError($"An unmapped file tag was returned by the database - {tagName}");
                                }
                            }
                            if (dr["FileTypeID"] != DBNull.Value && int.TryParse(dr["FileTypeID"].ToString(), out int intFileType) && intFileType > 0)
                            {
                                FileType type = new FileType();
                                type.Id = intFileType;
                                type.Name = dr["FileType"].ToString();
                                type.ImageIcon = dr["IconImage"].ToString();
                                record.FileType = type;
                            }
                            file.Add(record);
                        }
                    }
                    return file;
                }
            }
        }



        public async Task<FileDetails> GetFileDetails(int userId, int fileId)
        {
            DataTable dataTable = new DataTable();
            FileDetails details = null;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFileDetails", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = fileId;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        List<FileTag> fileTags = await GetFileTagsAll();

                        foreach (DataRow dr in dataTable.Rows)
                        {
                            details = new FileDetails()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                Author = dr["Author"].ToString(),
                                DateCreated = (dr["DateCreated"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DateCreated"].ToString())),
                                LastViewed = (dr["LastViewed"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastViewed"].ToString())),
                                WatchFlag = (dr["WatchFlag"] != DBNull.Value && Convert.ToBoolean(dr["WatchFlag"])),
                                FileURL = dr["FileURL"].ToString(),
                                Description = dr["Description"].ToString(),
                                PublishFlag = (dr["Published"] != DBNull.Value && Convert.ToBoolean(dr["Published"])),
                                PracticeOnly = (dr["PracticeOnly"] != DBNull.Value && Convert.ToBoolean(dr["PracticeOnly"])),
                                Tags = new List<FileTag>()
                            };

                            if (!string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
                            {
                                foreach (string tagName in dr["Tags"].ToString().Split(',').Select(t => Convert.ToString(t)))
                                {
                                    if (fileTags.Any(f => f.Name == tagName))
                                        details.Tags.Add(fileTags.First(f => f.Name == tagName));
                                    else
                                        _logger.LogError($"An unmapped file tag was returned by the database - {tagName}");
                                }
                            }

                            if (dr["FileTypeID"] != DBNull.Value && int.TryParse(dr["FileTypeID"].ToString(), out int intFileType) && intFileType > 0)
                            {
                                FileType type = new FileType();
                                type.Id = intFileType;
                                type.Name = dr["FileType"].ToString();
                                type.ImageIcon = dr["IconImage"].ToString();
                                details.FileType = type;
                            }
                            if (dr["ResourceTypeId"] != DBNull.Value && int.TryParse(dr["ResourceTypeId"].ToString(), out int intResourceType) && intResourceType > 0)
                            {
                                ResourceType type = new ResourceType();
                                type.Id = intResourceType;
                                type.Name = dr["ResourceType"].ToString();
                                details.ResourceType = type;
                            }
                            if (dr["InitiativeId"] != DBNull.Value && int.TryParse(dr["InitiativeId"].ToString(), out int intInitiative) && intInitiative > 0)
                            {
                                Initiative init = new Initiative();
                                init.Id = intInitiative;
                                init.Name = dr["Initiative"].ToString();
                                details.Initiative = init;
                            }

                            if (dr["WebContentPlacementId"] != DBNull.Value && int.TryParse(dr["WebContentPlacementId"].ToString(), out int intPlacement) && intPlacement > 0)
                            {
                                WebPlacement place = new WebPlacement();
                                place.Id = intPlacement;
                                place.Name = dr["WebPlacement"].ToString();
                                details.WebPlacement = place;
                            }
                        }


                    }
                }
            }

            return details;
        }


        public async Task<FileDetails> UpdateFileDetails(int userId, FileDetails fileDetails)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdateFile", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = fileDetails.Id;
                    sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = fileDetails.Name;
                    sqlCommand.Parameters.Add("@ResourceTypeId", SqlDbType.Int).Value = (fileDetails.ResourceType == null ? (int?)null : fileDetails.ResourceType.Id);
                    sqlCommand.Parameters.Add("@InitiativeId", SqlDbType.Int).Value = (fileDetails.Initiative == null ? (int?)null : fileDetails.Initiative.Id);
                    sqlCommand.Parameters.Add("@Description", SqlDbType.VarChar).Value = fileDetails.Description;
                    sqlCommand.Parameters.Add("@FileTypeID", SqlDbType.Int).Value = (fileDetails.FileType == null ? (int?)null : fileDetails.FileType.Id);
                    sqlCommand.Parameters.Add("@FileURL", SqlDbType.VarChar).Value = fileDetails.FileURL;
                    sqlCommand.Parameters.Add("@Author", SqlDbType.VarChar).Value = fileDetails.Author;
                    sqlCommand.Parameters.Add("@SearchTags", SqlDbType.VarChar).Value = string.Join(",", (from f in fileDetails.Tags select f.Name).ToArray());
                    sqlCommand.Parameters.Add("@Published", SqlDbType.Bit).Value = fileDetails.PublishFlag;
                    sqlCommand.Parameters.Add("@PracticeOnlyFlag", SqlDbType.Bit).Value = fileDetails.PracticeOnly;
                    sqlCommand.Parameters.Add("@CreateAlertFlag", SqlDbType.Bit).Value = fileDetails.CreateAlert;
                    sqlCommand.Parameters.Add("@WebContentPlacementId", SqlDbType.Int).Value = (fileDetails.WebPlacement == null ? (int?)null : fileDetails.WebPlacement.Id);

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    if ((bool)sqlCommand.ExecuteScalar() == true)
                    {
                        return await GetFileDetails(userId, fileDetails.Id);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public async Task<FileDetails> AddFileDetails(int userId, FileDetails fileDetails)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("spAddFile", sqlConnection))
                    {
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        sqlCommand.Parameters.Add("@Name", SqlDbType.VarChar).Value = fileDetails.Name;
                        sqlCommand.Parameters.Add("@ResourceTypeId", SqlDbType.Int).Value = (fileDetails.ResourceType == null ? (int?)null : fileDetails.ResourceType.Id);
                        sqlCommand.Parameters.Add("@InitiativeId", SqlDbType.Int).Value = (fileDetails.Initiative == null ? (int?)null : fileDetails.Initiative.Id);
                        sqlCommand.Parameters.Add("@Description", SqlDbType.VarChar).Value = fileDetails.Description;
                        sqlCommand.Parameters.Add("@FileTypeId", SqlDbType.Int).Value = (fileDetails.FileType == null ? (int?)null : fileDetails.FileType.Id);
                        sqlCommand.Parameters.Add("@FileURL", SqlDbType.VarChar).Value = fileDetails.FileURL;
                        sqlCommand.Parameters.Add("@Author", SqlDbType.VarChar).Value = fileDetails.Author;
                        sqlCommand.Parameters.Add("@SearchTags", SqlDbType.VarChar).Value = string.Join(",", (from f in fileDetails.Tags select f.Name).ToArray());
                        sqlCommand.Parameters.Add("@Published", SqlDbType.Bit).Value = fileDetails.PublishFlag;
                        sqlCommand.Parameters.Add("@PracticeOnlyFlag", SqlDbType.Bit).Value = fileDetails.PracticeOnly;
                        sqlCommand.Parameters.Add("@CreateAlertFlag", SqlDbType.Bit).Value = fileDetails.CreateAlert;
                        sqlCommand.Parameters.Add("@WebContentPlacementId", SqlDbType.Int).Value = (fileDetails.WebPlacement == null ? (int?)null : fileDetails.WebPlacement.Id);

                        await sqlConnection.OpenAsync();

                        //Execute Stored Procedure
                        int? fileId = (int)sqlCommand.ExecuteScalar();
                        if (fileId.HasValue && fileId > 0)
                        {
                            return await GetFileDetails(userId, fileId.Value);
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateFileWatch(int userId, int resourceId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spUpdateFileWatch", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }
        public async Task<bool> RemoveFile(int userId, int resourceId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spRemoveFile", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ResourceID", SqlDbType.Int).Value = resourceId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    return (bool)sqlCommand.ExecuteScalar();
                }
            }
        }


        public async Task<List<File>> ListFilesRecentlyCreated(int userId, bool toggleTop5)
        {
            DataTable dataTable = new DataTable();
            List<File> file = new List<File>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFilesMostRecent", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Top5", SqlDbType.Bit).Value = toggleTop5;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        //List<FileTag> fileTags = await GetFileTagsAll();
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new File()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                DateCreated = (dr["DateCreated"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DateCreated"].ToString())),
                                //LastViewed = (dr["LastViewed"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastViewed"].ToString())),
                                //WatchFlag = (dr["WatchFlag"] != DBNull.Value && Convert.ToBoolean(dr["WatchFlag"])),
                                //FileTypeId = (dr["FileTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FileTypeID"].ToString())),
                                FileURL = dr["FileURL"].ToString(),
                                //PublishFlag = (dr["Published"] != DBNull.Value && Convert.ToBoolean(dr["Published"])),
                                Description = dr["Description"].ToString(),
                                Tags = new List<FileTag>()
                            };
                            if (dr["FileTypeID"] != DBNull.Value && int.TryParse(dr["FileTypeID"].ToString(), out int intFileType) && intFileType > 0)
                            {
                                FileType type = new FileType();
                                type.Id = intFileType;
                                type.Name = dr["FileType"].ToString();
                                type.ImageIcon = dr["IconImage"].ToString();
                                record.FileType = type;
                            }
                            //if (!string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
                            //{
                            //    foreach (string tagName in dr["Tags"].ToString().Split(',').Select(t => Convert.ToString(t)))
                            //    {
                            //        if (fileTags.Any(f => f.Name == tagName))
                            //            record.Tags.Add(fileTags.First(f => f.Name == tagName));
                            //        else
                            //            _logger.LogError($"An unmapped file tag was returned by the database - {tagName}");
                            //    }
                            //}
                            file.Add(record);
                        }
                    }
                    return file;
                }
            }
        }


        public async Task<List<File>> ListFilesRecentlyViewed(int userId, bool toggleTop5)
        {
            DataTable dataTable = new DataTable();
            List<File> file = new List<File>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFilesRecentlyViewed", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Top5", SqlDbType.Bit).Value = toggleTop5;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        //List<FileTag> fileTags = await GetFileTagsAll();
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new File()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                //DateCreated = (dr["DateCreated"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DateCreated"].ToString())),
                                LastViewed = (dr["LastViewed"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastViewed"].ToString())),
                                //WatchFlag = (dr["WatchFlag"] != DBNull.Value && Convert.ToBoolean(dr["WatchFlag"])),
                                //FileTypeId = (dr["FileTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FileTypeID"].ToString())),
                                FileURL = dr["FileURL"].ToString(),
                                //PublishFlag = (dr["Published"] != DBNull.Value && Convert.ToBoolean(dr["Published"])),
                                Description = dr["Description"].ToString(),
                                Tags = new List<FileTag>()
                            };
                            if (dr["FileTypeID"] != DBNull.Value && int.TryParse(dr["FileTypeID"].ToString(), out int intFileType) && intFileType > 0)
                            {
                                FileType type = new FileType();
                                type.Id = intFileType;
                                type.Name = dr["FileType"].ToString();
                                type.ImageIcon = dr["IconImage"].ToString();
                                record.FileType = type;
                            }
                            //if (!string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
                            //{
                            //    foreach (string tagName in dr["Tags"].ToString().Split(',').Select(t => Convert.ToString(t)))
                            //    {
                            //        if (fileTags.Any(f => f.Name == tagName))
                            //            record.Tags.Add(fileTags.First(f => f.Name == tagName));
                            //        else
                            //            _logger.LogError($"An unmapped file tag was returned by the database - {tagName}");
                            //    }
                            //}
                            file.Add(record);
                        }
                    }
                    return file;
                }
            }
        }


        public async Task<List<PopularFile>> ListFilesMostPopular(int userId, bool toggleTop5)
        {
            DataTable dataTable = new DataTable();
            List<PopularFile> file = new List<PopularFile>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFilesMostPopular", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@Top5", SqlDbType.Bit).Value = toggleTop5;
                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        //List<FileTag> fileTags = await GetFileTagsAll();
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new PopularFile()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                //DateCreated = (dr["DateCreated"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["DateCreated"].ToString())),
                                //LastViewed = (dr["LastViewed"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(dr["LastViewed"].ToString())),
                                //WatchFlag = (dr["WatchFlag"] != DBNull.Value && Convert.ToBoolean(dr["WatchFlag"])),
                                //FileTypeId = (dr["FileTypeID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["FileTypeID"].ToString())),
                                ViewCount = (dr["Views"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Views"].ToString())),
                                FileURL = dr["FileURL"].ToString(),
                                //PublishFlag = (dr["Published"] != DBNull.Value && Convert.ToBoolean(dr["Published"])),
                                Description = dr["Description"].ToString(),
                                Tags = new List<FileTag>()
                            };
                            if (dr["FileTypeID"] != DBNull.Value && int.TryParse(dr["FileTypeID"].ToString(), out int intFileType) && intFileType > 0)
                            {
                                FileType type = new FileType();
                                type.Id = intFileType;
                                type.Name = dr["FileType"].ToString();
                                type.ImageIcon = dr["IconImage"].ToString();
                                record.FileType = type;
                            }
                            //if (!string.IsNullOrWhiteSpace(dr["Tags"].ToString()))
                            //{
                            //    foreach (string tagName in dr["Tags"].ToString().Split(',').Select(t => Convert.ToString(t)))
                            //    {
                            //        if (fileTags.Any(f => f.Name == tagName))
                            //            record.Tags.Add(fileTags.First(f => f.Name == tagName));
                            //        else
                            //            _logger.LogError($"An unmapped file tag was returned by the database - {tagName}");
                            //    }
                            //}
                            file.Add(record);
                        }
                    }
                    return file;
                }
            }
        }

        public async Task<List<FileTag>> GetFileTagsAll()
        {
            DataTable dataTable = new DataTable();
            List<FileTag> fileTags = new List<FileTag>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetResourceTags", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new FileTag()
                            {
                                Name = dr["Tag"].ToString()
                            };
                            fileTags.Add(record);
                        }
                    }
                    return fileTags;
                }
            }
        }

        public async Task<List<FileType>> GetFileTypesAll()
        {
            DataTable dataTable = new DataTable();
            List<FileType> fileTypes = new List<FileType>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetFileTypeList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new FileType()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString(),
                                ImageIcon = dr["IconImage"].ToString()
                            };
                            fileTypes.Add(record);
                        }
                    }
                    return fileTypes;
                }
            }
        }

        public async Task<List<Resource>> GetResourceAll()
        {
            DataTable dataTable = new DataTable();
            List<Resource> resources = new List<Resource>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetResourceTypeList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new Resource()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString()
                            };
                            resources.Add(record);
                        }
                    }
                    return resources;
                }
            }
        }

        public async Task<List<Initiative>> GetInitiativeAll()
        {
            DataTable dataTable = new DataTable();
            List<Initiative> initiatives = new List<Initiative>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetInitiativeList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new Initiative()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["ShortName"].ToString()
                            };
                            initiatives.Add(record);
                        }
                    }
                    return initiatives;
                }
            }
        }


        public async Task<List<WebPlacement>> GetQuicklinkPlacement()
        {
            DataTable dataTable = new DataTable();
            List<WebPlacement> placements = new List<WebPlacement>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spGetQuicklinkPlacementList", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    // Define the data adapter and fill the dataset
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            var record = new WebPlacement()
                            {
                                Id = (dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"].ToString())),
                                Name = dr["Name"].ToString()
                            };
                            placements.Add(record);
                        }
                    }
                    return placements;
                }
            }
        }

        public async Task MarkFileAction(int resourceId, int userId, int actionId)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_connectionStrings.PHODB))
            {
                using (SqlCommand sqlCommand = new SqlCommand("spPostFileAction", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlCommand.Parameters.Add("@ResourceId", SqlDbType.Int).Value = resourceId;
                    sqlCommand.Parameters.Add("@ActionId", SqlDbType.Int).Value = actionId;

                    await sqlConnection.OpenAsync();

                    //Execute Stored Procedure
                    sqlCommand.ExecuteNonQuery();

                }
            }
        }

    }
}
