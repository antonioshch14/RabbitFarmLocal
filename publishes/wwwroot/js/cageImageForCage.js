var canvas;
var ctx;
var CageId;
var htmlElem;
var cagesObj;
var cage; //single cage
var mr = 1; //margin btw cages
var mBtwRows = 4; //margin btw rows
var csw ; // cage single width
var csh; // cage single height
var rd; //distance btw rows
var fonNumbSize ;
var fontNumb ;
var fontLiversSize ;
var fontLivers;
var x;
var y;
var zLev; //ground level for row
var livers;
var dimOfCanv = {
    rows: 0,
    frames: 0,
    levels: 0
};
var cellsArray = [{
    xc: 0,
    yc: 0,
    xpc: 0,
    ypc: 0,
    id: ""
}]; //array of cells

function initiate() {
canvas = document.getElementById('cageImage');
ctx = canvas.getContext('2d');
CageId = document.getElementById('CageHighlight').innerHTML;
htmlElem = document.getElementById('jsonString').innerHTML;
cagesObj = JSON.parse(htmlElem);
csw = (canvas.width - (dimOfCanv.frames + 1) * mr - mr) / (dimOfCanv.frames + 1); // cage single width
    csh = (canvas.height - (dimOfCanv.rows * dimOfCanv.levels - 1) * mr - mBtwRows * (dimOfCanv.rows - 1)) / (dimOfCanv.rows * dimOfCanv.levels); // cage single height
    rd = (csh + mr) * dimOfCanv.levels - (csh) + mBtwRows; //distance btw rows
    fonNumbSize = csh * 0.7;
    fontNumb = fonNumbSize + 'px serif';
    fontLiversSize = (csh * 0.3);
    fontLivers = fontLiversSize + 'px serif';
    ctx.font = fontNumb;
    ctx.fillStyle = 'rgb(200, 0, 0)';
    ctx.lineWidth = 1;
}

initiate();

defineDimentions();

drawCages();

function drawCages() {
    for (let r = 0; r < cagesObj.Rows.length; r++) {
        zLev = rd * (r + 1) + csh * r - mBtwRows;
        ctx.fillStyle = 'rgb(0, 0, 0)';
        ctx.fillText(cagesObj.Rows[r].Nr, mr, zLev - csh * 0.2 + csh);
        // console.log(zLev);
        for (let f = 0; f < cagesObj.Rows[r].Frames.length; f++) {
            for (let l = 0; l < cagesObj.Rows[r].Frames[f].Levels.length; l++) {
                x = mr + f * (csw + mr) + csw + mr;
                y = zLev - l * (csh + mr);
                cage = cagesObj.Rows[r].Frames[f].Levels[l]; // one cage
                //if (CageId == parseInt(cagesObj.Rows[r].Frames[f].Levels[l].Id)) ctx.fillStyle = 'rgb(220,0,0)';
                //else ctx.fillStyle = 'rgb(192,192,192)';
                switch (cage.Oc) {
                    case 1:
                        ctx.fillStyle = 'rgb(240,10,10)'; //fem
                        break;
                    case 2:
                        ctx.fillStyle = 'rgb(10, 75, 240)'; //male
                        break;
                    case 4:
                        ctx.fillStyle = 'rgb(22, 224, 242)'; //fatt male
                        break;
                    case 3:
                        ctx.fillStyle = 'rgb(252, 151, 232)'; //fatt female
                        break;
                    default:
                        ctx.fillStyle = 'rgb(192,192,192)';

                };
                if (CageId == parseInt(cagesObj.Rows[r].Frames[f].Levels[l].Id)) {
                    let LWprev = ctx.lineWidth;
                    let SSPrev = ctx.strokeStyle;
                    ctx.lineWidth = 3;
                    ctx.strokeStyle = ctx.fillStyle;
                    ctx.strokeRect(x, y, csw, csh);
                    ctx.lineWidth = LWprev;
                    ctx.strokeStyle = SSPrev;
                } else ctx.fillRect(x, y, csw, csh);
                //save coordinates to the cell array
                let cellCoordinates = {
                    xc: x,
                    yc: y,
                    xpc: x + csw,
                    ypc: y + csh,
                    id: cagesObj.Rows[r].Frames[f].Levels[l].Id
                };
                cellsArray.push(cellCoordinates);

                ctx.fillStyle = 'rgb(0, 0, 0)';
                ctx.fillText(cagesObj.Rows[r].Frames[f].Levels[l].Id, x + mr, y - csh * 0.2 + csh);
                livers = parseInt(cage.Rbs);
                if (livers > 1 || cage.Oc == '3' || cage.Oc == '4') { // write livers
                    // console.log(cage.Oc + "  " + cage.Rbs);
                    ctx.font = fontLivers;
                    ctx.fillText(cage.Rbs, x + mr + csw - fontLiversSize, y + fontLiversSize * 1.2);
                    ctx.font = fontNumb;
                }
            }
        }
        //draw line btw rows
        ctx.beginPath(); // Start a new path
        ctx.moveTo(0, zLev + csh + mr + mBtwRows / 2); // Move the pen to (x,y)
        ctx.lineTo(canvas.width, zLev + csh + mr + mBtwRows / 2); // Draw a line to (x,y)
        ctx.stroke(); // Render the path
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
