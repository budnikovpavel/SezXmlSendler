using System.Data;

namespace SezXmlSendler.Model.Interfaces
{
    /// <summary>
    /// Интерфейс класса, который производит заполнение сериализуемого элемента данными из таблицы
    /// </summary>
    public interface IFillOnTable
    {
        /// <summary>
        ///  Метод заполнения сериализуемого элемента данными из  из таблицы 
        /// </summary>
        /// <param name="tbl">таблица с данными</param>
        public void FillOnTable(DataTable tbl);
    }
}