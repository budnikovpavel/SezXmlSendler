using System.Data;

namespace SezXmlSendler.Model.Interfaces
{
    /// <summary>
    /// Интерфейс класса, который производит заполнение сериализуемого элемента данными из строки DataRow
    /// </summary>
    public interface IFillOnRow
    {
        /// <summary>
        /// Метод заполнения сериализуемого элемента данными из строки DataRow
        /// </summary>
        /// <param name="row">строка с данными</param>
        public void FillOnRow(DataRow row);
    }
}
