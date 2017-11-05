using System;
using System.Collections;
using AmCharts.Windows.Column;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class Charts
    {
        private readonly EntityType _entityType;

        public Charts(EntityType entityType)
        {
            try
            {
                InitializeComponent();

                _entityType = entityType;
                IList lstGenres = Util.GetService(entityType).GetTypesName();
                CreateSlice(lstGenres);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void CreateSlice(IList lstGenres)
        {
            int i = 1;
            ColumnChartGraph graph = new ColumnChartGraph();
            foreach (string item in lstGenres)
            {

                ColumnDataPoint objSlice = new ColumnDataPoint();
                objSlice.SeriesID = i;
                objSlice.Title = item;
                objSlice.LabelText = item;
                objSlice.Value = Util.GetService(_entityType).GetCountByType(item);
                i++;


                if (objSlice.Value > 0)
                    graph.DataItems.Add(objSlice);
            }

            columnChart.Graphs.Add(graph);
        }
    }
}