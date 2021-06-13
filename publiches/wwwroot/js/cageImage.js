
var canvas = document.getElementById('cageImage');
var ctx = canvas.getContext('2d');

var dimOfCanv = {
    rows: 0,
    frames: 0,
    levels: 0
};

var CageId = document.getElementById('CageHighlight').innerHTML;
var htmlElem = document.getElementById('jsonString').innerHTML;
var cagesObj = JSON.parse(htmlElem);
defineDimentions();
var mr = 1; //margin btw cages
var csw = (canvas.width - (dimOfCanv.frames + 1) * mr - mr) / (dimOfCanv.frames + 1);  // cage sige width
var csh = (canvas.height - (dimOfCanv.rows * dimOfCanv.levels - 1) * mr) / (dimOfCanv.rows * dimOfCanv.levels); // cage sige height

var rd = (csh + mr) * dimOfCanv.levels - (csh); //distance btw rows
//console.log(csh, canvas.height, dimOfCanv.rows, dimOfCanv.levels, rd);
ctx.font = (csh * 0.7) + 'px serif';
ctx.fillStyle = 'rgb(200, 0, 0)';
var x;
var y;
var zLev; //ground level for row
for (let r = 0; r < cagesObj.Rows.length; r++) {
    zLev = rd * (r + 1) + csh * r;
    ctx.fillStyle = 'rgb(0, 0, 0)';
    ctx.fillText(cagesObj.Rows[r].Nr, mr, zLev - csh * 0.2 + csh);
   // console.log(zLev);
    for (let f = 0; f < cagesObj.Rows[r].Frames.length; f++) {
        for (let l = 0; l < cagesObj.Rows[r].Frames[f].Levels.length; l++) {
            x = mr + f * (csw + mr) + csw + mr;
            y = zLev - l * (csh + mr);
            if (CageId == parseInt(cagesObj.Rows[r].Frames[f].Levels[l].Id)) ctx.fillStyle = 'rgb(220,0,0)';
            else ctx.fillStyle = 'rgb(192,192,192)';
            ctx.fillRect(x, y, csw, csh);
            ctx.fillStyle = 'rgb(0, 0, 0)';
            ctx.fillText(cagesObj.Rows[r].Frames[f].Levels[l].Id, x + mr, y - csh * 0.2 + csh);
        }
    }
}

function defineDimentions() {
    dimOfCanv.rows = cagesObj.Rows.length;
    for (let r = 0; r < cagesObj.Rows.length; r++) {
        if (cagesObj.Rows[r].Frames.length > dimOfCanv.frames) dimOfCanv.frames = cagesObj.Rows[r].Frames.length;
        for (let f = 0; f < cagesObj.Rows[r].Frames.length; f++) {
            if (cagesObj.Rows[r].Frames[f].Levels.length > dimOfCanv.levels) dimOfCanv.levels = cagesObj.Rows[r].Frames[f].Levels.length;
        }
    }
}