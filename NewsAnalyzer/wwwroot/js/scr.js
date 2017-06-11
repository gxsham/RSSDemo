var hzBarData = [];

var chart = AmCharts.makeChart("chartdiv", {
    "type": "serial",
    "theme": "light",
    "rotate": true,
    "marginBottom": 50,
    "dataProvider": hzBarData,
    "startDuration": 1,
    "graphs": [{
        "fillAlphas": 0.4,
        "lineAlpha": 0.2,
        "type": "column",
        "valueField": "negative",
        "title": "Male",
        "labelText": "[[value]]",
        "clustered": false,
        "labelFunction": function (item) {
            return Math.abs(item.values.value);
        },
        "balloonFunction": function (item) {
            return item.category + ": " + Math.abs(item.values.value) + "%";
        }
    }, {
        "fillAlphas": 0.8,
        "lineAlpha": 0.2,
        "type": "column",
        "valueField": "positive",
        "title": "Female",
        "labelText": "[[value]]",
        "clustered": false,
        "labelFunction": function (item) {
            return Math.abs(item.values.value);
        },
        "balloonFunction": function (item) {
            return Math.abs(item.values.value) + "%";
        }
    }],
    "categoryField": "age",
    "categoryAxis": {
        "gridPosition": "start",
        "gridAlpha": 0.2,
        "axisAlpha": 0
    },
    "valueAxes": [{
        "gridAlpha": 0,
        "ignoreAxisWidth": true,
        "labelFunction": function (value) {
            return "";
        },
        "guides": [{
            "value": 0,
            "lineAlpha": 0.2
        }]
    }],
    "balloon": {
        "fixedPosition": true
    },
    "chartCursor": {
        "valueBalloonsEnabled": false,
        "cursorAlpha": 0.05,
        "fullWidth": true
    },
    "allLabels": [
        {
            "text": "Negative",
            "x": "28%",
            "y": "97%",
            "bold": true,
            "align": "middle"
        }, {
            "text": "Positive",
            "x": "75%",
            "y": "97%",
            "bold": true,
            "align": "middle"
        }],
    "export": {
        "enabled": true
    }
});

$(document).ready(function () {

    $("input[type='checkbox'][id^='check_']").on('click', function () {
        var check = $(this),
            id = check.attr("id"),
            idnum = id.split('_')[1];
        var txt = parseInt($("label[id='sentiment_" + idnum + "']").text());
        var pos = txt === 0 ? -1 : 1;
        var neg = txt === 1 ? 1 : -1;
        var portal = $("label[id='portal_" + idnum + "']").text();

        if (check.is(":checked")) {
            
            var indx = hzBarData.findIndex(function(val){
                return val.age === portal;
            });

            if (indx !== -1) {
                if (txt == 0) {
                    hzBarData[indx].negative += -1;
                } else {
                    hzBarData[indx].positive += 1;
                }
            } else {
                hzBarData.push({
                    age: portal,
                    positive: txt === 1 ? 1 : 0,
                    negative: txt === 0 ? -1 : 0
                });
            }
        } else {
            var indx = hzBarData.findIndex(function (val) {
                return val.age === portal;
            });

            if (txt === 0) {
                hzBarData[indx].negative -= -1;
            } else {
                hzBarData[indx].positive -= 1;
            }
            if (hzBarData[indx].positive === 0 && hzBarData[indx].negative === 0) {
                hzBarData.splice(indx, 1);
            }
        }
        chart.validateData();
    })
});