var width = 960,
    height = 600,
    active = d3.select(null);
var iscancel = false;
var iszoom = false;

var color = ["#8dd3c7", "#ffffb3", "#bebada", "#fb8072", "#80b1d3", "#fdb462", "#b3de69", "#fccde5", "#fbb4ae", "#b3cde3", "#ccebc5", "#decbe4", "#fed9a6", "#ffffcc", "#e5d8bd", "#fddaec", "#e41a1c", "#377eb8", "#4daf4a", "#984ea3", "#ff7f00", "#ffff33", "#a65628", "#f781bf"];

var projection = d3.geoMercator()
    .scale(1500)
    .translate([width / 20, height / 3]);

var tooltip = d3.select("body")
    .append("div")
    .attr("class", "tooltip")
    .style("position", "absolute")
    .style("z-index", "10")
    .style("visibility", "hidden");

var zoom = d3.zoom()
    .scaleExtent([1, 8])
    .on("zoom", zoomed);

var path = d3.geoPath()
    .projection(projection);

var svg = d3.select("#map").append("svg")
    .attr("width", width)
    .attr("height", height)
    .on("click", stopped, true);

svg.append("rect")
    .attr("class", "background")
    .attr("width", width)
    .attr("height", height)
    .on("click", reset);

var g = svg.append("g");

svg.call(zoom);

var tooltip = d3.select("body")
    .append("div")
    .attr("class", "nvtooltip xy-tooltip")
    .attr("id", "tooltip")
    .style("position", "absolute")
    .style("z-index", "10")
    .style("visibility", "hidden");
d3.json("/Resources/COD_TOPO.json", function (error, cod) {
    if (error) throw error;
    g.selectAll("path")
        .data(topojson.feature(cod, cod.objects.Territoires).features)
        .enter().append("path")
        .attr("d", path)
        .attr("class", "feature")
        .attr("stateFipsCode", function (d) { return d.properties.GID_1.slice(4); })
        .on("mouseover", function (d) { if (generatetable(d)) { return tooltip.style("visibility", "visible"); } })
        .on("mousemove", function () { return tooltip.style("top", (d3.event.pageY - 10) + "px").style("left", (d3.event.pageX + 10) + "px"); })
        .on("mouseout", function () { return tooltip.style("visibility", "hidden"); })
        .on("click", clicked);

    g.append("path")
        .datum(topojson.mesh(cod, cod.objects.Provinces, function (a, b) { return a !== b; }))
        .attr("class", "mesh")
        .attr("d", path)
        .on("mouseover", console.log("Provinces"))
        .on("click", function (d) { console.log("Clicked Provinces:" + d); });

    g.selectAll("text")
        .data(topojson.feature(cod, cod.objects.Provinces).features)
        .enter()
        .append("svg:text")
        .text(function (d) { return d.properties.NAME_1.slice(0, 4); })
        .attr("x", function (d) { return path.centroid(d)[0]; })
        .attr("y", function (d) { return path.centroid(d)[1]; })
        .attr("text-anchor", "middle")
        .attr('font-size', '6pt');
});

var hover = function (d) {

    console.log("hover");
    tooltip.html("");
    tooltip.style("visibility", "visible")
        .style("border", "4px solid ")
        .style("fill", "red");

    tooltip.append("h2")
        .text("Province: " + d.properties.NAME_1);
    tooltip.append("h3")
        .text("Enroles: " + d.NombreEnrole);
    tooltip.append("div")
        .text("Code: " + d.properties.GID_1);
    tooltip.append("div")
        .text("Type: " + d.properties.ENGTYPE_1);
};

function clicked(d) {
    iszoom = true;
    iscancel = false;
    $("#btncancel").prop('disabled', false);



    var stateElem = $("[stateFipsCode = " + d.properties.GID_1.slice(4) + "]");
    // stateElem.attr("fill", "#1a80c4");
    debugger
    if (active.node() === this) {
        stateElem.removeClass("active");
        return reset();
    }
    else {
        active.classed("active", false);//else}
        $('.feature').removeClass('active');
        stateElem.addClass("active");
    }
    active = d3.select(this).classed("active", true);

    var bounds = path.bounds(d),
        dx = bounds[1][0] - bounds[0][0],
        dy = bounds[1][1] - bounds[0][1],
        x = (bounds[0][0] + bounds[1][0]) / 2,
        y = (bounds[0][1] + bounds[1][1]) / 2,
        scale = Math.max(1, Math.min(8, 0.9 / Math.max(dx / width, dy / height))),
        translate = [width / 2 - scale * x, height / 2 - scale * y];

    svg.transition().duration(750).call(zoom.transform, d3.zoomIdentity.translate(translate[0], translate[1]).scale(scale));
}

function reset() {
    active.classed("active", false);
    active = d3.select(null);
    $('.feature').removeClass('active');
    iscancel = false;
    svg.transition().duration(750).call(zoom.transform, d3.zoomIdentity);
}

function zoomed() {
    g.style("stroke-width", 1.5 / d3.event.transform.k + "px");
    g.attr("transform", d3.event.transform);
}

function stopped() {
    if (d3.event.defaultPrevented) d3.event.stopPropagation();
}

function generatetable(res) {
    if (iszoom) {
        $('#header').text(res.properties.NAME_2);
    }
    else {
        $('#header').text(res.properties.NAME_1);
    }
    var tltipdiv = document.getElementById('tooltip');
    var table = document.getElementById('nvtable');
    
    tltipdiv.appendChild(table);
    return true;
}
function activearea() {
    if (iscancel) {
        iscancel = false;
        $("#btncancel").prop('disabled', false);
        iszoom = true;
    }
    else {
        iscancel = true;
        $("#btncancel").prop('disabled', true);
        iszoom = false;
        reset();
    }
}
