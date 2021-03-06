﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Shared.Dto.BucketList;
using Shared.Dto;
using BucketListDataProvider.Constants;

namespace BucketListDataProvider
{
    public partial class BucketListData : IBucketListData
    {
        public string[] GetBucketListV2(string userName, string sortString)
        {
            IList<BucketListItem> listItems = GetListItemsV2(userName, sortString);
            string[] items = null;
            int ctr = 0;
            int ctrDisplay = 1;

            if (listItems == null || listItems.Count < 1)
            {
                items = new string[1];
                items[0] = "No Items";
            }
            else
            {
                items = new string[listItems.Count];

                foreach (BucketListItem bli in listItems)
                {
                    items[ctr] = "," + bli.Name
                                    + "," + bli.Created.ToString("MM/dd/yyyy").Trim('0')
                                        + "," + bli.Category
                                            + "," + bli.GetIntStringAchievedValue()
                                              + "," + bli.Latitude
                                                 + "," + bli.Longitude
                                                    + "," + bli.Id.ToString();
                    ctr++;
                    ctrDisplay++;
                }
            }

            return items;
        }

        public bool UpsertBucketListItemV2(string[] bucketListItems)
        {
            bool goodDbAction = false;
            bool Achieved = false;

            string ListItemName = bucketListItems[0];
            DateTime Created = this.GetSafeDateTime(bucketListItems[1]);
            string Category = bucketListItems[2];
            string AchievedStr = bucketListItems[3];
            decimal Latitude = this.GetSafeDecimal(bucketListItems[4]);
            decimal Longitude = this.GetSafeDecimal(bucketListItems[5]);

            int BucketListItemId = 0;
            if (!string.IsNullOrEmpty(bucketListItems[6]))
                BucketListItemId = this.GetSafeInt(bucketListItems[6]);

            string UserName = bucketListItems[7];

            if (!string.IsNullOrEmpty(AchievedStr) && AchievedStr.Equals("1"))
                Achieved = true;

            goodDbAction = UpsertBucketListItemV2(ListItemName, Created, Category, Achieved, BucketListItemId, UserName, Latitude, Longitude);

            return goodDbAction;
        }

        private IList<BucketListItem> GetListItemsV2(string userName, string sortString)
        {
            IList<BucketListItem> listItems = new List<BucketListItem>();
            BucketListItem bli = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rdr = null;

            try
            {
                conn = new SqlConnection(connectionString);
                cmd = conn.CreateCommand();

                if (!string.IsNullOrEmpty(sortString))
                {
                    //HACK - Update in a later version
                    if (sortString.Equals(" order by Category"))
                        cmd.CommandText = BucketListSqlV2.GET_BUCKET_LIST + "order by CategorySortOrder";
                    else if (sortString.Equals(" order by Category desc"))
                        cmd.CommandText = BucketListSqlV2.GET_BUCKET_LIST + "order by CategorySortOrder desc";
                    else
                        cmd.CommandText = BucketListSqlV2.GET_BUCKET_LIST + sortString;
                }
                else
                    cmd.CommandText = BucketListSqlV2.GET_BUCKET_LIST;

                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@userName", userName));

                cmd.Connection.Open();

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    bli = new BucketListItem();

                    bli.Name = GetSafeString(rdr[0]);
                    bli.Created = GetSafeDateTime(rdr[1]);
                    bli.Category = GetSafeString(rdr[2]);
                    bli.Achieved = GetSafeBool(rdr[3]);
                    bli.Id = GetSafeInt(rdr[4]);
                    bli.Latitude = GetSafeDecimal(rdr[5]);
                    bli.Longitude = GetSafeDecimal(rdr[6]);

                    listItems.Add(bli);
                }
            }
            catch (Exception ex)
            {
                LogMsg(ex.Message);
            }
            finally
            {
                CloseDbObjects(conn, cmd, null);
            }

            return listItems;
        }

        private bool UpsertBucketListItemV2(string ListItemName,
                                            DateTime Created,
                                            string Category,
                                            bool Achieved,
                                            int BucketListItemId,
                                            string UserName, 
                                            decimal Latitude,
                                            decimal Longitude)
        {
            IList<BucketListItem> listItems = new List<BucketListItem>();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            bool goodDbAction = false;

            try
            {
                conn = new SqlConnection(connectionString);
                cmd = conn.CreateCommand();
                cmd.CommandText = BucketListSqlV2.UPSERT_BUCKET_LIST_ITEM;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@BucketListItemId", BucketListItemId));
                cmd.Parameters.Add(new SqlParameter("@ListItemName", ListItemName));
                cmd.Parameters.Add(new SqlParameter("@Created", Created));
                cmd.Parameters.Add(new SqlParameter("@Category", Category));
                cmd.Parameters.Add(new SqlParameter("@Achieved", Achieved));
                cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                cmd.Parameters.Add(new SqlParameter("@Latitude", Latitude));
                cmd.Parameters.Add(new SqlParameter("@Longitude", Longitude));

                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                goodDbAction = true;
            }
            catch (Exception ex)
            {
                LogMsg(ex.Message);
            }
            finally
            {
                CloseDbObjects(conn, cmd, null);
            }

            return goodDbAction;
        }
    }
}
