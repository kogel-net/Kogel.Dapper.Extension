using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Kogel.Dapper.Extension.Model
{
  
    public class JoinAssTable
    {
        private JoinAction _action;
        private JoinMode _joinMode;
        private string _rightTabName;
        private string _leftTabName;
        private string _rightAssName;
        private string _leftAssName;
        private Type _tableType;
        private string _joinSql;
        public DynamicParameters _params;



        public JoinAction Action { get => _action; set => _action = value; }
        public JoinMode JoinMode { get => _joinMode; set => _joinMode = value; }
        public string RightTabName { get => _rightTabName; set => _rightTabName = value; }
        public string LeftTabName { get => _leftTabName; set => _leftTabName = value; }
        public string RightAssName { get => _rightAssName; set => _rightAssName = value; }
        public string LeftAssName { get => _leftAssName; set => _leftAssName = value; }
        public Type TableType { get => _tableType; set => _tableType = value; }
        public string JoinSql { get => _joinSql; set => _joinSql = value; }
    }

    public enum JoinAction
    {
        defaults,
        sqlJoin,//sql查询
    }

    public enum JoinMode
    {
        LEFT,//左连接
        RIGHT,//右连接
        INNER,//内连接
        FULL,//全连接
    }
}
