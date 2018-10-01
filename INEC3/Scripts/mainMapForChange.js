var width = 1000,
    height = 800,
    active = d3.select(null);

var color = ["#8dd3c7", "#ffffb3", "#bebada", "#fb8072", "#80b1d3", "#fdb462", "#b3de69", "#fccde5", "#fbb4ae", "#b3cde3", "#ccebc5", "#decbe4", "#fed9a6", "#ffffcc", "#e5d8bd", "#fddaec", "#e41a1c", "#377eb8", "#4daf4a", "#984ea3", "#ff7f00", "#ffff33", "#a65628", "#f781bf"];

var projection = d3.geoMercator()
    .scale(1700)
    .translate([width / 35, height / 4]);

var tooltip = d3.select("body")
    .append("div")
    .attr("class", "tooltip")
    .style("position", "absolute")
    .style("z-index", "10")
    .style("visibility", "hidden");

var hover = function (d) {

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

    //   d3.selectAll("path")
    //     .style("opacity", 0.7)
    //     .style("stroke", null);
    //  d3.select(this)
    //     .style("opacity", 1)
    //     .style("stroke", "#fff")
    //     .raise();
    // d3.selectAll("feature")
    //   .style("opacity", 0);

};

// var out = function(d) {
//
//   tooltip.style("visibility", "hidden")
//   d3.selectAll("path")
//     .style("opacity", 1);
//   d3.selectAll("feature")
//     .style("opacity", 1);
//
//   };

var zoom = d3.zoom()
    .scaleExtent([1, 8])
    .on("zoom", zoomed);

var path = d3.geoPath()
    .projection(projection);

var svg = d3.select("body").append("svg")
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

// d3.queue()
//     .defer(d3.json, "COD_TOPO.v1.json")
//     .defer(d3.csv, "province.csv")
//     .await(ready);

// function ready(error, cod) {
//   if (error) throw error;
//
//   g.selectAll("path")
//       .data(topojson.feature(cod, cod.objects.Provinces).features)
//       .enter().append("path")
//       .attr("d", path)
//       .attr("class", "feature")
//       .on("mouseover", hover)
//       .on("click", clicked);
//
//   g.append("path")
//       .datum(topojson.mesh(cod, cod.objects.Provinces, function(a, b) { return a !== b; }))
//       .attr("class", "mesh")
//       .attr("d", path);
//
// };

d3.json("/Resources/COD_TOPO.json", function (error, cod) {
    if (error) throw error;

    g.selectAll("path")
        .data(topojson.feature(cod, cod.objects.Provinces).features)
        .enter().append("path")
        .attr("d", path)
        .attr("class", "feature")
        .attr("fill", function (d) {
            return color;
        })
        .on("mouseover", hover)
        // .on("mouseout", out)
        .on("click", clicked);

    g.append("path")
        .datum(topojson.mesh(cod, cod.objects.Provinces, function (a, b) { return a !== b; }))
        .attr("class", "mesh")
        .attr("d", path);


    console.log(cod);
});


function clicked(d) {
    if (active.node() === this) return reset();
    active.classed("active", false);

    active = d3.select(this)
        .classed("active", true);

    var bounds = path.bounds(d),
        dx = bounds[1][0] - bounds[0][0],
        dy = bounds[1][1] - bounds[0][1],
        x = (bounds[0][0] + bounds[1][0]) / 2,
        y = (bounds[0][1] + bounds[1][1]) / 2,
        scale = Math.max(1, Math.min(8, 0.9 / Math.max(dx / width, dy / height))),
        translate = [width / 2 - scale * x, height / 2 - scale * y];

    svg.transition()
        .duration(750)
        .call(zoom.transform, d3.zoomIdentity.translate(translate[0], translate[1]).scale(scale));
}

function reset() {
    active.classed("active", false);
    active = d3.select(null);

    svg.transition()
        .duration(750)
        .call(zoom.transform, d3.zoomIdentity);
}

function zoomed() {
    g.style("stroke-width", 1.5 / d3.event.transform.k + "px");
    g.attr("transform", d3.event.transform);
}
// If the drag behavior prevents the default click,
// also stop propagation so we don’t click-to-zoom.
function stopped() {
    if (d3.event.defaultPrevented) d3.event.stopPropagation();
}
