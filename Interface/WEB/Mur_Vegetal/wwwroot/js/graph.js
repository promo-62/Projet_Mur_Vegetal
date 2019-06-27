var tab_temp = [
    {
        date: 1,
        value: 7
    },
    {
        date: 2,
        value: 10
    },
    {
        date: 3,
        value: 9
    },
    {
        date: 4,
        value: 22
    },
    {
        date: 5,
        value: 12
    },
    {
        date: 6,
        value: 14
    },
    {
        date: 7,
        value: 34
    },
    {
        date: 8,
        value: 43
    },
    {
        date: 9,
        value: 15
    },
    {
        date: 10,
        value: 8
    },
    {
        date: 11,
        value: 10
    },
    {
        date: 12,
        value: 19
    }
];


var graph = {
    lineChart(divname, tab, state) {
        // state pour Staticscreen (= 0) et Usersite (= 1)
        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        var chart = am4core.create(divname, am4charts.XYChart);

        chart.data = tab;

        // Create axes
        var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
        dateAxis.renderer.minGridDistance = 60;

        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

        // Create series
        var series = chart.series.push(new am4charts.LineSeries());
        series.dataFields.valueY = "value";
        series.dataFields.dateX = "date";
        series.tooltipText = "{value}"

        series.tooltip.pointerOrientation = "vertical";

        chart.cursor = new am4charts.XYCursor();
        chart.cursor.snapToSeries = series;
        chart.cursor.xAxis = dateAxis;

        //chart.scrollbarY = new am4core.Scrollbar();

        if (state == 0) {
            chart.hideCredits = true;
            dateAxis.renderer.labels.template.disabled = true;
            dateAxis.tooltip.disabled = true;
            $("document").ready(function(){ //Hide credits on charts
                $("g[aria-labelledby]").hide();
              });

        } else {
            chart.scrollbarX = new am4core.Scrollbar();

        }

    },
    columnChart(divname, tab) {
        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        var chart = am4core.create(divname, am4charts.XYChart);
        chart.hiddenState.properties.opacity = 0; // this creates initial fade-in

        chart.data = tab;
        var max = Math.max.apply(Math, tab.map(function (o) { return o.degree; }));

        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.dataFields.category = "date";
        categoryAxis.renderer.minGridDistance = 40;
        categoryAxis.fontSize = 11;

        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxis.min = 0;
        valueAxis.max = max;
        valueAxis.strictMinMax = true;
        valueAxis.renderer.minGridDistance = 30;
        // axis break
        var axisBreak = valueAxis.axisBreaks.create();
        axisBreak.startValue = 2100;
        axisBreak.endValue = 22900;
        axisBreak.breakSize = 0.005;

        // make break expand on hover
        var hoverState = axisBreak.states.create("hover");
        hoverState.properties.breakSize = 1;
        hoverState.properties.opacity = 0.1;
        hoverState.transitionDuration = 1500;

        axisBreak.defaultState.transitionDuration = 1000;

        var series = chart.series.push(new am4charts.ColumnSeries());
        series.dataFields.categoryX = "date";
        series.dataFields.valueY = "value";
        series.columns.template.tooltipText = "{valueY.value}";
        series.columns.template.tooltipY = 0;
        series.columns.template.strokeOpacity = 0;

        // as by default columns of the same series are of the same color, we add adapter which takes colors from chart.colors color set
        series.columns.template.adapter.add("fill", function (fill, target) {
            return chart.colors.getIndex(target.dataItem.index);
        });

    }

}
