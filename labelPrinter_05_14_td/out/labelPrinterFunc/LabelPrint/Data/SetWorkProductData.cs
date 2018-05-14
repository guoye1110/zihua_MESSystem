using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace LabelPrint.Data
{

    public class MiscCutProductData
    {
        public String ProductCode;
        public String Width;
        public String RecipeCode;
        public String Fixture;
        public String CustomerName;
        public String BatchNo;
        public String RawMaterialCode;


        public String productLength;
        public String productName;
        public String productWeight;

        public String PlateRollPerLay;
        public String PlateLayer;
        public String PlateRollNum;


        public int PlateNo;
        public int CurRollNum;
        public String MiscIndexInOneProcess;
        public int SetWorkProductIndex;

        public int LittleRollNo = 1;
        public TextBox tb_PlateRollPerLay;
        public TextBox tb_PlateLayer;
        public TextBox tb_PlateRollNum;
        public TextBox tb_RawMaterialCode;
        public TextBox tb_CustomerName;
        public TextBox tb_BatchNo;
        public TextBox tb_Recipe;
        public TextBox tb_PlateNo;
        private const int LittleRollMax = 14;
        private const int Max = 999;

        int[] LittleRollIndexArray = new int[LittleRollMax];
        public int UsedCount = 0;

        public MiscCutProductData()
        {
            Reset();
        }
        public Boolean AddProductCodeComBoxIndex(int index)
        {
            for (int i = 0; i < LittleRollMax; i++)
            {
                if (LittleRollIndexArray[i] == Max)
                {
                    LittleRollIndexArray[i] = index;
                    UsedCount++;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            for (int i = 0; i < LittleRollMax; i++)
            {
                LittleRollIndexArray[i] = Max;
            }
        }

        public Boolean RemoveOldInfoByComboIndex(int index)
        {
            for (int i = 0; i < LittleRollMax; i++)
            {
                if (LittleRollIndexArray[i] == index)
                {
                    LittleRollIndexArray[i] = Max;
                    UsedCount--;
                    return true;
                }
            }
            return false;
        }
        public Boolean FindInfoByComboIndex(int index)
        {
            for (int i = 0; i < LittleRollMax; i++)
            {
                if (LittleRollIndexArray[i] == index)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean IsComBoxCountEmpty()
        {
            if (UsedCount == 0)
                return true;
            else
                return false;
        }

        public int FindLowestComboIndex()
        {
            int little = Max;
            for (int i = 0; i < LittleRollMax; i++)
            {
                if (LittleRollIndexArray[i] == Max)
                    continue;

                if(LittleRollIndexArray[i] < little)
                {
                    little = LittleRollIndexArray[i];
                }
            }
            if (little == Max)
                return Max;
            else
                return little;
        }
        public void UpdateUI()
        {
            tb_RawMaterialCode.Text = RawMaterialCode;
            tb_CustomerName.Text = CustomerName;
            tb_BatchNo.Text = BatchNo;
            tb_PlateNo.Text = PlateNo.ToString();
            tb_Recipe.Text = RecipeCode;

            tb_PlateRollPerLay.Text = PlateRollPerLay;
            tb_PlateLayer.Text = PlateLayer;
            tb_PlateRollNum.Text = PlateRollNum;

        //public TextBox ;
        //public TextBox tb_Recipe;
        //public TextBox tb_PlateNo;
    }
    };

    public class SetWorkProductData
    {
        private const int ProductTypeCount = 3;

        MiscCutProductData[] MiscProductList = new MiscCutProductData[ProductTypeCount];
        int TotalProductInMisc = 0;



        public SetWorkProductData()
        {
            //TotalProductInMisc = 0;
            Reset();
        }

        public int GetTotalLittleRollInOneRound()
        {
            int total = 0;
                           
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null)
                {
                    total += MiscProductList[i].UsedCount;
                }
            }
            return total;
        }
        public void Reset()
        {
            TotalProductInMisc = 0;
            for (int i = 0; i < ProductTypeCount; i++)
            {
                MiscProductList[i] = null;
            }

        }
        public Boolean IsEmptyMiscCutProductDataAvailable()
        {
            return (TotalProductInMisc != ProductTypeCount);
        }
        public Boolean IsMiscCutProductDataAvailable(String ProductCode)
        {
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null && (MiscProductList[i].ProductCode == ProductCode))
                {
                    return true;
                }
            }
            if (TotalProductInMisc >= ProductTypeCount)
                return false;
            else
                return true;

        }
        //Search and find the productcode type index;
        public MiscCutProductData GetMiscCutProductData(String ProductCode)
        {
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null && (MiscProductList[i].ProductCode == ProductCode))
                {
                    return MiscProductList[i];
                }
            }

            if (TotalProductInMisc >= ProductTypeCount)
                return null;
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] == null)
                {
                    MiscProductList[i] = new MiscCutProductData();
                    MiscProductList[i].SetWorkProductIndex = i;
                    TotalProductInMisc++;
                    return MiscProductList[i];
                }
            }
            return null;
        }

        public MiscCutProductData SearchMiscCutProductDataWithoutCreation(String ProductCode)
        {
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null && (MiscProductList[i].ProductCode == ProductCode))
                {
                    return MiscProductList[i];
                }
            }

            return null;
        }
        //Search the index;
        public int GetMiscCutProductIndex(String ProductCode)
        {
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null && (MiscProductList[i].ProductCode == ProductCode))
                {
                    return i;
                }
            }
            return -1;
        }

        #region Get Info by Index
        public String GetProductCodeByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return null;
            return MiscProductList[index].ProductCode;
        }

        public String GetCustomerNameByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return null;
            return MiscProductList[index].CustomerName;
        }

        public String GetBatchNoByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return null;
            return MiscProductList[index].BatchNo;
        }

        public String GetRecipeCodeByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return null;
            return MiscProductList[index].RecipeCode;
        }

        public String GetFixtureByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return null;
            return MiscProductList[index].Fixture;
        }

        public int GetPlateNoByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return -1;
            return MiscProductList[index].PlateNo;
        }

        public int GetCurRollNumByIndex(int index)
        {
            if (index >= ProductTypeCount || index < 0)
                return -1;
            return MiscProductList[index].CurRollNum;
        }

        #endregion Get Info by Index
        public String GetCustomerNameByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return null;

            return MiscProductList[index].CustomerName;
        }

        public String GetBatchNoByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return null;
            return MiscProductList[index].BatchNo;
        }

        public String GetRecipeCodeByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return null;
            return MiscProductList[index].RecipeCode;
        }

        public String GetFixtureByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return null;
            return MiscProductList[index].Fixture;
        }

        public int GetPlateNoByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return -1;
            return MiscProductList[index].PlateNo;
        }

        public int GetCurRollNumByProductCode(String productcode)
        {
            int index;
            index = GetMiscCutProductIndex(productcode);
            if (index < 0)
                return -1;
            return MiscProductList[index].CurRollNum;
        }

        public Boolean RemoveOldInfoByComboIndex(int ComIndex)
        {
            int index = -1;
            for (int i =0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] == null)
                    continue;
                if(MiscProductList[i].RemoveOldInfoByComboIndex(ComIndex))
                {
                    if (MiscProductList[i].IsComBoxCountEmpty())
                    {
                        index = i;
                        //for (int k = i; k < TotalProductInMisc-1; k++)
                        //{
                        //    MiscProductList[k] = MiscProductList[k+1];
                        //    MiscProductList[k].index = k;
                        //}
                        //MiscProductList[TotalProductInMisc-1] =null;

                        MiscProductList[i].tb_BatchNo.BackColor = Color.White;
                        MiscProductList[i].tb_PlateNo.BackColor = Color.White;
                         MiscProductList[i].tb_CustomerName.BackColor = Color.White;
                        MiscProductList[i].tb_Recipe.BackColor = Color.White;
                        MiscProductList[i].tb_RawMaterialCode.BackColor = Color.White;
                        MiscProductList[i] = null;
                        TotalProductInMisc--;
                    }
                    return true;
                }
            }
            return false;
        }
        public MiscCutProductData FindInfoByComboIndex(int ComIndex)
        {

            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] == null)
                    continue;
                if (MiscProductList[i].FindInfoByComboIndex(ComIndex))
                {

                    return  MiscProductList[i];

                }
            }
            return null;
        }
        public int FindInfoIndexByComboIndex(int ComIndex)
        {

            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] == null)
                    continue;
                if (MiscProductList[i].FindInfoByComboIndex(ComIndex))
                {

                    return i;

                }
            }
            return -1;
        }

        public  void sortMiscCutProductData()
        {
            MiscCutProductData temp = null;
            for (int i = 0; i < ProductTypeCount; i++)
            {
                for (int j= (ProductTypeCount-1); j>i; j--)
                {
                    if ((MiscProductList[j] == null) && MiscProductList[j -1] == null)
                        continue;
                    if ((MiscProductList[j] == null) && MiscProductList[j -1] != null)
                    {
                        continue;
                    }

                    if (((MiscProductList[j]!=null)&& (MiscProductList[j-1] == null))
                        ||(MiscProductList[j].FindLowestComboIndex()< MiscProductList[j-1].FindLowestComboIndex())
                        )
                    {
                        temp = MiscProductList[j - 1];
                        MiscProductList[j - 1] = MiscProductList[j];
                        MiscProductList[j] = temp;
                    }
                }
            }
            
            return;
        }


        public void UpdateUI(TextBox[] CustomerNames, TextBox[]BatchNo, TextBox[]Recipe, TextBox []PlateNo,TextBox[] RawMaterialCode,TextBox[]PlateRollPerLay, TextBox[] PlateLayer, TextBox[] PlateRollNum)
        {
            sortMiscCutProductData();
            for (int i = 0; i < ProductTypeCount; i++)
            {
                if (MiscProductList[i] != null )
                {
                    //tb_CustomerName
                    MiscProductList[i].tb_RawMaterialCode = RawMaterialCode[i];
                    MiscProductList[i].tb_CustomerName = CustomerNames[i];
                    MiscProductList[i].tb_BatchNo = BatchNo[i];
                    MiscProductList[i].tb_Recipe = Recipe[i];
                    MiscProductList[i].tb_PlateNo = PlateNo[i];

                    MiscProductList[i].tb_PlateRollPerLay = PlateRollPerLay[i];
                    MiscProductList[i].tb_PlateLayer = PlateLayer[i];
                    MiscProductList[i].tb_PlateRollNum = PlateRollNum[i];

                    //tb_BatchNos[i].Text = null;
                    //tb_PlateNos[i].Text = null;
                    //tb_Recipes[i]

                    MiscProductList[i].UpdateUI();
                }else
                {

                    RawMaterialCode[i].Text = null;
                    CustomerNames[i].Text = null;
                    BatchNo[i].Text = null;
                    Recipe[i].Text = null;
                    PlateNo[i].Text = null;
                    PlateRollPerLay[i].Text = null;
                    PlateLayer[i].Text = null;
                    PlateRollNum[i].Text = null;
                }
            }
        }

    }
}
