using Microsoft.AspNetCore.Http;
using BookAPI.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BookAPI.Models;
using Ubiety.Dns.Core;
using Microsoft.AspNetCore.Mvc;
using ReadExcel_API.Controllers;

namespace ReadExcel_API.Controllers
{
    [RoutePrefix("Api/Excel")]
    public class ReadExcelController : ApiController
    {
        [System.Web.Http.Route("ReadFile")]
        [System.Web.Http.HttpPost]
        public string ReadFile()
        {
            try
            {
                #region Variable Declaration
                string message = "";
                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                #endregion

                #region Save Book Detail From Excel
                using (dbBookEntities objEntity = new dbCodingvilaEntities())
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        Inputfile = httpRequest.Files[0];
                        FileStream = Inputfile.InputStream;

                        if (Inputfile != null && FileStream != null)
                        {
                            if (Inputfile.FileName.EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                            }
                            else if (Inputfile.FileName.EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                            }
                            else
                                message = "The file format is not supported.";

                            dsexcelRecords = reader.AsDataSet();
                            reader.Close();

                            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                            {
                                DataTable dtBookRecords = dsexcelRecords.Tables[0];
                                for (int i = 0; i < dtBookRecords.Rows.Count; i++)
                                {
                                    Book objBook = new Book();
                                    objBook.Id = Convert.ToInt32(dtBookRecords.Rows[i][0]);
                                    objBook.Title = Convert.ToString(dtBookRecords.Rows[i][1]);
                                    objBook.Author = Convert.ToString(dtBookRecords.Rows[i][2]);
                                    objBook.Description = Convert.ToString(dtBookRecords.Rows[i][3]);
                                    objEntity.Books.Add(objBook);
                                }

                                int output = objEntity.SaveChanges();
                                if (output > 0)
                                    message = "The Excel file has been successfully uploaded.";
                                else
                                    message = "Something Went Wrong!, The Excel file uploaded has fiald.";
                            }
                            else
                                message = "Selected file is empty.";
                        }
                        else
                            message = "Invalid File.";
                    }
                    else
                        ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return message;
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}