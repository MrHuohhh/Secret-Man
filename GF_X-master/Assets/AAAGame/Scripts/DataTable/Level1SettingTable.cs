//------------------------------------------------------------
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：__DATA_TABLE_CREATE_TIME__
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

[System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = false)]
/// <summary>
/// Level1SettingTable
/// </summary>
public class Level1SettingTable : DataRowBase
{
	private int m_Id = 0;
	/// <summary>
    /// 阶段
    /// </summary>
    public override int Id
    {
        get { return m_Id; }
    }

        /// <summary>
        /// 阶段分数
        /// </summary>
        public int Target
        {
            get;
            private set;
        }

        /// <summary>
        /// 点击分数/点击
        /// </summary>
        public int ClickScore
        {
            get;
            private set;
        }

        /// <summary>
        /// 长按分数/帧
        /// </summary>
        public int TapScore
        {
            get;
            private set;
        }

        /// <summary>
        /// 损失分数/帧
        /// </summary>
        public int ScoreLost
        {
            get;
            private set;
        }

        /// <summary>
        /// 来电固定间隔时间判定来电间隔判定概率
        /// </summary>
        public int Event_Boss_OpenCD
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Event_Boss_OpenProp
        {
            get;
            private set;
        }

        /// <summary>
        /// 挂断固定间隔时间判定挂断电话判定概率
        /// </summary>
        public int Event_Boss_CloseCD
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Event_Boss_CloseProp
        {
            get;
            private set;
        }

        /// <summary>
        /// 敲门判定间隔时间
        /// </summary>
        public int Event_Door_OpenCD
        {
            get;
            private set;
        }

        /// <summary>
        /// 触发敲门判定概率
        /// </summary>
        public int Event_Door_OpenProp
        {
            get;
            private set;
        }

        /// <summary>
        /// 撤离预告时间与概率
        /// </summary>
        public float[][] Event_Door_PreTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 开门事件概率
        /// </summary>
        public int Event_Door_TypeProp
        {
            get;
            private set;
        }

        /// <summary>
        /// 敌人生命值
        /// </summary>
        public int EnemyLives
        {
            get;
            private set;
        }

        /// <summary>
        /// 敌人移动时间
        /// </summary>
        public int EnemyMoveTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 敌人伤害
        /// </summary>
        public int Damage
        {
            get;
            private set;
        }

        /// <summary>
        /// 间隔判定时间
        /// </summary>
        public int Event_Windows_OpenCD
        {
            get;
            private set;
        }

        /// <summary>
        /// 触发概率
        /// </summary>
        public int Event_Windows_OpenProp
        {
            get;
            private set;
        }

        /// <summary>
        /// 关窗判定时间
        /// </summary>
        public int Event_Windows_CloseCD
        {
            get;
            private set;
        }

        /// <summary>
        /// 关窗触发概率
        /// </summary>
        public int Event_Windows_CloseProp
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            Target = int.Parse(columnStrings[index++]);
            ClickScore = int.Parse(columnStrings[index++]);
            TapScore = int.Parse(columnStrings[index++]);
            ScoreLost = int.Parse(columnStrings[index++]);
            Event_Boss_OpenCD = int.Parse(columnStrings[index++]);
            Event_Boss_OpenProp = int.Parse(columnStrings[index++]);
            Event_Boss_CloseCD = int.Parse(columnStrings[index++]);
            Event_Boss_CloseProp = int.Parse(columnStrings[index++]);
            Event_Door_OpenCD = int.Parse(columnStrings[index++]);
            Event_Door_OpenProp = int.Parse(columnStrings[index++]);
            Event_Door_PreTime = DataTableExtension.Parse2DArray<float>(columnStrings[index++]);
            Event_Door_TypeProp = int.Parse(columnStrings[index++]);
            EnemyLives = int.Parse(columnStrings[index++]);
            EnemyMoveTime = int.Parse(columnStrings[index++]);
            Damage = int.Parse(columnStrings[index++]);
            Event_Windows_OpenCD = int.Parse(columnStrings[index++]);
            Event_Windows_OpenProp = int.Parse(columnStrings[index++]);
            Event_Windows_CloseCD = int.Parse(columnStrings[index++]);
            Event_Windows_CloseProp = int.Parse(columnStrings[index++]);
            index++;

            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    Target = binaryReader.Read7BitEncodedInt32();
                    ClickScore = binaryReader.Read7BitEncodedInt32();
                    TapScore = binaryReader.Read7BitEncodedInt32();
                    ScoreLost = binaryReader.Read7BitEncodedInt32();
                    Event_Boss_OpenCD = binaryReader.Read7BitEncodedInt32();
                    Event_Boss_OpenProp = binaryReader.Read7BitEncodedInt32();
                    Event_Boss_CloseCD = binaryReader.Read7BitEncodedInt32();
                    Event_Boss_CloseProp = binaryReader.Read7BitEncodedInt32();
                    Event_Door_OpenCD = binaryReader.Read7BitEncodedInt32();
                    Event_Door_OpenProp = binaryReader.Read7BitEncodedInt32();
                    Event_Door_PreTime = DataTableExtension.Parse2DArray<float>(binaryReader.ReadString());
                    Event_Door_TypeProp = binaryReader.Read7BitEncodedInt32();
                    EnemyLives = binaryReader.Read7BitEncodedInt32();
                    EnemyMoveTime = binaryReader.Read7BitEncodedInt32();
                    Damage = binaryReader.Read7BitEncodedInt32();
                    Event_Windows_OpenCD = binaryReader.Read7BitEncodedInt32();
                    Event_Windows_OpenProp = binaryReader.Read7BitEncodedInt32();
                    Event_Windows_CloseCD = binaryReader.Read7BitEncodedInt32();
                    Event_Windows_CloseProp = binaryReader.Read7BitEncodedInt32();
                }
            }

            return true;
        }

//__DATA_TABLE_PROPERTY_ARRAY__
}
