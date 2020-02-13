using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AriaPM.Models;

namespace AriaPM.Services
{
    public class PalletService : IPalletService<Pallet>
    {
        List<Pallet> pallets;
        List<PickList> status;
        List<PickList> stores;
        List<PickList> categories;

        public async Task<bool> AddUpdatePalletAsync(Pallet pallet)
        {
            int insertedId = 0;
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select MAX(id) From pallet; Select MAX(id) From pallet_item", null);

            if (result != null)
            {
                int PalletId = pallet.Id;
                int maxPalletItemId = Convert.ToInt32(((DataSet)result).Tables[1].Rows[0][0] is DBNull ? 0 : ((DataSet)result).Tables[1].Rows[0][0]);
                string query = "";

                if (pallet.Id > 0)
                {
                    query = "Update pallet set status =@Status,wrapped_date=@WrappedDate,sent_date=@SentDate,freight_company=@FreightCompany," +
                        "con_number =@ConNumber,packed_by=@PackedBy,other_notes=@OtherNotes,received_date=@ReceivedDate,received_by=@ReceivedBy," +
                        "store_id=@StoreId,Contents=@Contents,wrapped_by=@WrappedBy,built_by=@BuiltBy, category=@Category, pallet_type=@PalletType, weight=@Weight, supplier=@Supplier where id =@Id;";
                }
                else
                {
                    query = "Insert INTO pallet(id,status,wrapped_date,sent_date,freight_company,con_number,packed_by,other_notes,received_date,received_by,store_id,Contents,wrapped_by,built_by,date_created, category, pallet_type, weight, supplier)values(" +
                                     "@Id, @Status, @WrappedDate, @SentDate, @FreightCompany,@ConNumber,@PackedBy,@OtherNotes,@ReceivedDate,@ReceivedBy,@StoreId,@Contents,@WrappedBy,@BuiltBy,NOW(),@Category, @PalletType, @Weight, @Supplier)";
                    PalletId = Convert.ToInt32(((DataSet)result).Tables[0].Rows[0][0] is DBNull ? 0 : ((DataSet)result).Tables[0].Rows[0][0]);
                    PalletId += 1;
                }
                ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
                dbParams.Add(new Params() { Name = "Id", Value = PalletId });
                dbParams.Add(new Params() { Name = "Status", Value = pallet.Status });
                dbParams.Add(new Params() { Name = "WrappedDate", Value = pallet.WrappedDate });
                dbParams.Add(new Params() { Name = "SentDate", Value = pallet.SentDate });
                dbParams.Add(new Params() { Name = "FreightCompany", Value = pallet.FreightCompany });
                dbParams.Add(new Params() { Name = "ConNumber", Value = pallet.ConNumber });
                dbParams.Add(new Params() { Name = "PackedBy", Value = pallet.PackedBy });
                dbParams.Add(new Params() { Name = "OtherNotes", Value = pallet.OtherNotes });
                dbParams.Add(new Params() { Name = "ReceivedDate", Value = pallet.ReceivedDate });
                dbParams.Add(new Params() { Name = "ReceivedBy", Value = pallet.ReceivedBy });
                dbParams.Add(new Params() { Name = "StoreId", Value = pallet.StoreId });
                dbParams.Add(new Params() { Name = "Contents", Value = pallet.Contents });
                dbParams.Add(new Params() { Name = "WrappedBy", Value = pallet.WrappedBy });
                dbParams.Add(new Params() { Name = "Category", Value = pallet.Category });
                dbParams.Add(new Params() { Name = "PalletType", Value = pallet.PalletType });
                dbParams.Add(new Params() { Name = "Weight", Value = pallet.Weight });
                dbParams.Add(new Params() { Name = "Supplier", Value = pallet.Supplier });
                dbParams.Add(new Params() { Name = "BuiltBy", Value = pallet.BuiltBy });

                insertedId = dBConnection.ExecuteNonQuery(query, dbParams);

                if (pallet.PalletItem != null && pallet.PalletItem.Count > 0 && insertedId > 0)
                {
                    string query1 = "Insert INTO pallet_item(id,quantity,barcode,description,`outer`, `inner`,wrapper_name,pallet_id)values";
                    ObservableCollection<Params> dbParams1 = new ObservableCollection<Params>();

                    for (int i = 0; i < pallet.PalletItem.Count; i++)
                    {
                        query1 = query1 + "(@Id" + i + ", @Quantity" + i + ", @Barcode" + i + ", @Description" + i + ", @Outer" + i + ",@Inner" + i + ",@WrapperName" + i + ",@Palletid" + i + ")";
                        if (i < pallet.PalletItem.Count - 1)
                            query1 = query1 + ",";
                        dbParams1.Add(new Params() { Name = "Id" + i + "", Value = maxPalletItemId + i + 1 });
                        dbParams1.Add(new Params() { Name = "Quantity" + i + "", Value = pallet.PalletItem[i].Quantity });
                        dbParams1.Add(new Params() { Name = "Barcode" + i + "", Value = pallet.PalletItem[i].Barcode });
                        dbParams1.Add(new Params() { Name = "Description" + i + "", Value = pallet.PalletItem[i].Description });
                        dbParams1.Add(new Params() { Name = "Outer" + i + "", Value = pallet.PalletItem[i].Outer });
                        dbParams1.Add(new Params() { Name = "Inner" + i + "", Value = pallet.PalletItem[i].Inner });
                        dbParams1.Add(new Params() { Name = "WrapperName" + i + "", Value = pallet.PalletItem[i].WrapperName });
                        dbParams1.Add(new Params() { Name = "Palletid" + i + "", Value = PalletId });
                    }

                    insertedId = dBConnection.ExecuteNonQuery(query1, dbParams1);
                }
            }
            return await Task.FromResult(insertedId > 0 ? true : false);
        }

        public async Task<bool> UpdatePalletStatusAsync(string status, int palletId)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Update pallet set status =@Status where id =@Id;";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "Id", Value = palletId });
            dbParams.Add(new Params() { Name = "Status", Value = status });
            int insertedId = dBConnection.ExecuteNonQuery(query, dbParams);
            return await Task.FromResult(insertedId > 0 ? true : false);
        }

        public async Task<bool> DeletePalletAsync(int palletId)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Delete from pallet_item where pallet_id =@Id;";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "Id", Value = palletId });
            int result = dBConnection.ExecuteNonQuery(query, dbParams);
            query = "Delete from pallet where id =@Id;";
            result = dBConnection.ExecuteNonQuery(query, dbParams);
            return await Task.FromResult(result > 0 ? true : false);
        }

        public async Task<Pallet> GetPalletAsync(int id)
        {
            DBConnection dBConnection = new DBConnection();
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "PalletId", Value = id });
            var result = dBConnection.GetQueryResult("Select * From pallet where id =@PalletId; Select * from pallet_item where pallet_id = @PalletId", dbParams);
            Pallet pallet = new Pallet();

            if (result != null)
            {
                DataTable dataTable = new DataTable();
                dataTable = ((DataSet)result).Tables[1];
                List<PalletItem> palletItems = new List<PalletItem>();
                PalletItem palletItem = null;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    palletItem = new PalletItem()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Quantity = Convert.ToString(dataTable.Rows[i]["quantity"]),
                        Barcode = Convert.ToString(dataTable.Rows[i]["barcode"]),
                        Description = Convert.ToString(dataTable.Rows[i]["description"]),
                        Outer = Convert.ToString(dataTable.Rows[i]["outer"]),
                        Inner = Convert.ToString(dataTable.Rows[i]["inner"]),
                        WrapperName = Convert.ToString(dataTable.Rows[i]["wrapper_name"]),
                        Palletid = Convert.ToInt32(dataTable.Rows[i]["pallet_id"])
                    };

                    palletItems.Add(palletItem);
                };

                dataTable = ((DataSet)result).Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    pallet = new Pallet()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Status = Convert.ToString(dataTable.Rows[i]["status"]),
                        WrappedDate = Convert.ToString(dataTable.Rows[i]["wrapped_date"]),
                        SentDate = Convert.ToString(dataTable.Rows[i]["sent_date"]),
                        FreightCompany = Convert.ToString(dataTable.Rows[i]["freight_company"]),
                        ConNumber = Convert.ToString(dataTable.Rows[i]["con_number"]),
                        PackedBy = Convert.ToString(dataTable.Rows[i]["packed_by"]),
                        OtherNotes = Convert.ToString(dataTable.Rows[i]["other_notes"]),
                        ReceivedDate = Convert.ToString(dataTable.Rows[i]["received_date"]),
                        ReceivedBy = Convert.ToString(dataTable.Rows[i]["received_by"]),
                        StoreId = Convert.ToInt32(dataTable.Rows[i]["store_id"]),
                        Contents = Convert.ToString(dataTable.Rows[i]["Contents"]),
                        WrappedBy = Convert.ToString(dataTable.Rows[i]["wrapped_by"]),
                        BuiltBy = Convert.ToString(dataTable.Rows[i]["built_by"]),
                        Category = Convert.ToString(dataTable.Rows[i]["category"]),
                        PalletType = Convert.ToString(dataTable.Rows[i]["pallet_type"]),
                        Weight = Convert.ToString(dataTable.Rows[i]["weight"]),
                        Supplier = Convert.ToString(dataTable.Rows[i]["supplier"]),
                        PalletItem = palletItems
                    };
                };
            }
            return await Task.FromResult(pallet);
        }

        public async Task<IEnumerable<Pallet>> GetItemsAsync(Pallet palletItem)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Select id, status, pallet_type, weight, supplier, wrapped_date, sent_date, con_number, received_date, store_id, category From pallet";
            string countQuery = "Select count(1) as count From pallet";
            ObservableCollection<Params> dbParams = null;

            if (palletItem != null)
            {
                string subquery = string.Empty;
                dbParams = new ObservableCollection<Params>();

                if (palletItem.StoreId != null)
                {
                    dbParams.Add(new Params() { Name = "store_id", Value = palletItem.StoreId });
                    subquery = "store_id = @store_id";
                }

                if (!string.IsNullOrWhiteSpace(palletItem.Status?.Trim()))
                {
                    dbParams.Add(new Params() { Name = "status", Value = palletItem.Status });

                    if (!string.IsNullOrWhiteSpace(subquery))
                        subquery = subquery + " AND ";

                    subquery = subquery + "status = @status";
                }

                if (!string.IsNullOrWhiteSpace(palletItem.Category?.Trim()))
                {
                    dbParams.Add(new Params() { Name = "category", Value = palletItem.Category });

                    if (!string.IsNullOrWhiteSpace(subquery))
                        subquery = subquery + " AND ";

                    subquery = subquery + "category = @category";
                }

                if (!string.IsNullOrWhiteSpace(palletItem.Barcode?.Trim()))
                {
                    dbParams.Add(new Params() { Name = "barcode", Value = palletItem.Barcode });

                    if (!string.IsNullOrWhiteSpace(subquery))
                        subquery = subquery + " AND ";

                    subquery = subquery + "barcode = @barcode";
                }

                if (!string.IsNullOrWhiteSpace(palletItem.Description?.Trim()))
                {
                    dbParams.Add(new Params() { Name = "Contents", Value = palletItem.Description });

                    if (!string.IsNullOrWhiteSpace(subquery))
                        subquery = subquery + " AND ";

                    subquery = subquery + "Contents = @Contents";
                }

                if (palletItem.Id > 0)
                {
                    dbParams.Add(new Params() { Name = "id", Value = palletItem.Id });

                    if (!string.IsNullOrWhiteSpace(subquery))
                        subquery = subquery + " AND ";

                    subquery = subquery + "id = @id";
                }

                if (dbParams != null && dbParams.Count > 0)
                {
                    query = query + " where " + subquery;
                    countQuery = countQuery + " where " + subquery;
                }
            }

            if (palletItem.PageNumber == 0)
            {
                palletItem.PageNumber = 1;
            }

            query = query + " order by id limit " + (palletItem.PageNumber <= 1 ? 0 : (palletItem.PageNumber - 1) * 20) + ",20" + ";";
            query = query + countQuery;
            var result = dBConnection.GetQueryResult(query, dbParams);
            pallets = new List<Pallet>();

            if (result != null)
            {
                DataTable dataTable = ((DataSet)result).Tables[0];
                int total = Convert.ToInt32(((DataSet)result).Tables[1].Rows[0]["count"]);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Pallet item = new Pallet()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Status = Convert.ToString(dataTable.Rows[i]["status"]),
                        ConNumber = Convert.ToString(dataTable.Rows[i]["con_number"]),
                        StoreRequest = Convert.ToString(dataTable.Rows[i]["store_id"]),
                        PalletType = Convert.ToString(dataTable.Rows[i]["pallet_type"]),
                        Supplier = Convert.ToString(dataTable.Rows[i]["supplier"]),
                        Category = Convert.ToString(dataTable.Rows[i]["category"]),
                        WrappedDate = Convert.ToString(dataTable.Rows[i]["wrapped_date"]),
                        SentDate = Convert.ToString(dataTable.Rows[i]["sent_date"]),
                        ReceivedDate = Convert.ToString(dataTable.Rows[i]["received_date"]),
                        Weight = Convert.ToString(dataTable.Rows[i]["weight"]),
                        Total = total
                    };

                    pallets.Add(item);
                };
            }

            return await Task.FromResult(pallets);
        }

        public async Task<IEnumerable<PickList>> GetStoresAsync()
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select id, store_name From store_name ", null);
            stores = new List<PickList>();

            if (result != null)
            {
                DataTable dataTable = new DataTable();
                dataTable = ((DataSet)result).Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    PickList store = new PickList()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Name = Convert.ToString(dataTable.Rows[i]["store_name"])
                    };

                    stores.Add(store);

                };
            }
            return await Task.FromResult(stores.OrderBy(s => s.Name));
        }

        public async Task<IEnumerable<PickList>> GetStatusAsync()
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select id, status_name From pallet_status ", null);
            status = new List<PickList>();

            if (result != null)
            {
                DataTable dataTable = ((DataSet)result).Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    PickList s = new PickList()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Name = Convert.ToString(dataTable.Rows[i]["status_name"])
                    };

                    status.Add(s);
                };
            }
            return await Task.FromResult(status.OrderBy(s => s.Name));
        }

        public async Task<IEnumerable<PickList>> GetCategoriesAsync()
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select category_id, category_name From pallet_categories ", null);
            categories = new List<PickList>();

            if (result != null)
            {
                DataTable dataTable = ((DataSet)result).Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    PickList category = new PickList()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["category_id"]),
                        Name = Convert.ToString(dataTable.Rows[i]["category_name"])
                    };

                    categories.Add(category);
                };
            }
            return await Task.FromResult(categories.OrderBy(c => c.Name));
        }

        public async Task<PalletMaster> GetPalletMaterDataAsync()
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select id, store_name From store_name; Select id, status_name From pallet_status; Select category_id, category_name From pallet_categories; " +
                "Select builder_id, builder_name From pallet_builders; Select shipper_id, shipper_name From pallet_shippers; Select supplies_id, supplies_name From pallet_supplies;" +
                "Select wrapper_id, wrapper_name From pallet_wrappers; Select id, type From pallet_type;", null);


            PalletMaster palletMaster = new PalletMaster();

            if (result != null)
            {
                DataSet dataSet = (DataSet)result;

                palletMaster.Stores = CreatePickListData(dataSet.Tables[0], "id", "store_name");
                palletMaster.Status = CreatePickListData(dataSet.Tables[1], "ID", "status_name");
                palletMaster.Categories = CreatePickListData(dataSet.Tables[2], "category_id", "category_name");
                palletMaster.Builders = CreatePickListData(dataSet.Tables[3], "builder_id", "builder_name");
                palletMaster.Shippers = CreatePickListData(dataSet.Tables[4], "shipper_id", "shipper_name");
                palletMaster.Suppliers = CreatePickListData(dataSet.Tables[5], "supplies_id", "supplies_name");
                palletMaster.Wrappers = CreatePickListData(dataSet.Tables[6], "wrapper_id", "wrapper_name");
                palletMaster.PalletTypes = CreatePickListData(dataSet.Tables[7], "id", "type");
            }
            return await Task.FromResult(palletMaster);
        }

        private List<PickList> CreatePickListData(DataTable dataTable, string id, string name)
        {
            List<PickList> pickList = new List<PickList>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                PickList item = new PickList()
                {
                    Id = Convert.ToInt32(dataTable.Rows[i][id]),
                    Name = Convert.ToString(dataTable.Rows[i][name])
                };
                pickList.Add(item);
            };

            return pickList.OrderBy(p => p.Name).ToList();
        }

        public async Task<bool> AddPalletItemAsync(PalletItem palletItem)
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select MAX(id) From pallet_item", null);
            int insertedId = 0;

            if (result != null)
            {
                int maxPalletItemId = Convert.ToInt32(((DataSet)result).Tables[0].Rows[0][0] is DBNull ? 0 : ((DataSet)result).Tables[0].Rows[0][0]);
                string query = "Insert INTO pallet_item(id,quantity,barcode,description,`outer`, `inner`,wrapper_name,pallet_id) values " +
                    "(@Id" + ", @Quantity" + ", @Barcode" + ", @Description" + ", @Outer" + ",@Inner" + ",@WrapperName" + ",@Palletid" + ")";
                ObservableCollection<Params> dbParams1 = new ObservableCollection<Params>();
                dbParams1.Add(new Params() { Name = "Id" + "", Value = maxPalletItemId + 1 });
                dbParams1.Add(new Params() { Name = "Quantity" + "", Value = palletItem.Quantity });
                dbParams1.Add(new Params() { Name = "Barcode", Value = palletItem.Barcode });
                dbParams1.Add(new Params() { Name = "Description", Value = palletItem.Description });
                dbParams1.Add(new Params() { Name = "Outer", Value = palletItem.Outer });
                dbParams1.Add(new Params() { Name = "Inner", Value = palletItem.Inner });
                dbParams1.Add(new Params() { Name = "WrapperName", Value = palletItem.WrapperName });
                dbParams1.Add(new Params() { Name = "Palletid", Value = palletItem.Palletid });
                insertedId = dBConnection.ExecuteNonQuery(query, dbParams1);
            }

            return await Task.FromResult(insertedId > 0 ? true : false);
        }

        public async Task<IEnumerable<Pallet>> GetPalletsByStatusAsync(string status)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Select id, status, pallet_type, weight, supplier, wrapped_date, sent_date, con_number, received_date, store_id, category From pallet where status = @status order by id;";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "status", Value = status });
            var result = dBConnection.GetQueryResult(query, dbParams);
            pallets = new List<Pallet>();

            if (result != null)
            {
                DataTable dataTable = ((DataSet)result).Tables[0];

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Pallet item = new Pallet()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[i]["id"]),
                        Status = Convert.ToString(dataTable.Rows[i]["status"]),
                        ConNumber = Convert.ToString(dataTable.Rows[i]["con_number"]),
                        StoreRequest = Convert.ToString(dataTable.Rows[i]["store_id"]),
                        PalletType = Convert.ToString(dataTable.Rows[i]["pallet_type"]),
                        Supplier = Convert.ToString(dataTable.Rows[i]["supplier"]),
                        Category = Convert.ToString(dataTable.Rows[i]["category"]),
                        WrappedDate = Convert.ToString(dataTable.Rows[i]["wrapped_date"]),
                        SentDate = Convert.ToString(dataTable.Rows[i]["sent_date"]),
                        ReceivedDate = Convert.ToString(dataTable.Rows[i]["received_date"]),
                        Weight = Convert.ToString(dataTable.Rows[i]["weight"])
                    };

                    pallets.Add(item);
                };
            }

            return await Task.FromResult(pallets);
        }

        public async Task<List<PickList>> GetShipperDataAsync()
        {
            DBConnection dBConnection = new DBConnection();
            var result = dBConnection.GetQueryResult("Select shipper_id, shipper_name From pallet_shippers;", null);
            List<PickList> Shippers = new List<PickList>();

            if (result != null)
            {
                DataSet dataSet = (DataSet)result;
                Shippers = CreatePickListData(dataSet.Tables[0], "shipper_id", "shipper_name");
            }

            return await Task.FromResult(Shippers);
        }

        public async Task<bool> UpdatePalletStatusWithShipperAsync(List<int> palletIds, string status, string shipper, string conNumber)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Update pallet set status =@Status, freight_company = @freight_company, con_number = @con_number where id in ( ";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            int count = 0;
            foreach (var id in palletIds)
            {
                query = query + "@Id" + count+ ",";
                dbParams.Add(new Params() { Name = "Id" + count, Value = id });
                count++;
            }

            query = query.TrimEnd(',') + ");";

            dbParams.Add(new Params() { Name = "Status", Value = status });
            dbParams.Add(new Params() { Name = "con_number", Value = conNumber });
            dbParams.Add(new Params() { Name = "freight_company", Value = shipper });
            int insertedId = dBConnection.ExecuteNonQuery(query, dbParams);
            return await Task.FromResult(insertedId > 0 ? true : false);
        }

        public async Task<bool> DeletePalletItemAsync(int id)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Delete from pallet_item where id =@Id;";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "Id", Value = id });
            int result = dBConnection.ExecuteNonQuery(query, dbParams);
            return await Task.FromResult(result > 0 ? true : false);
        }

        public async Task<bool> UpdatePalletItemAsync(PalletItem item)
        {
            DBConnection dBConnection = new DBConnection();
            string query = "Update pallet_item set quantity = @quantity,barcode=@barcode,description=@description,`outer`=@outer, `inner`=@inner,wrapper_name=@wrapper_name where id =@Id;";
            ObservableCollection<Params> dbParams = new ObservableCollection<Params>();
            dbParams.Add(new Params() { Name = "id", Value = item.Id });
            dbParams.Add(new Params() { Name = "quantity", Value = item.Quantity});
            dbParams.Add(new Params() { Name = "barcode", Value = item.Barcode});
            dbParams.Add(new Params() { Name = "description", Value = item.Description });
            dbParams.Add(new Params() { Name = "outer", Value = item.Outer });
            dbParams.Add(new Params() { Name = "inner", Value = item.Inner });
            dbParams.Add(new Params() { Name = "wrapper_name", Value = item.WrapperName});
            int result = dBConnection.ExecuteNonQuery(query, dbParams);
            return await Task.FromResult(result > 0 ? true : false);
        }
    }
}