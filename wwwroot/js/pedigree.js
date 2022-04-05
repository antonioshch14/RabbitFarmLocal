function drawPedigree() {
    var canvas = document.getElementById('pedigree_drawing');
    var ctx = canvas.getContext('2d');
    var dimOfCanv = {
        rows: 0,
        columns: 0,
        goLeft: 0,
        goRight: 0
    };
    const DrawLeft = Symbol("left");
    const DrawRight = Symbol("right");
    const DrawCenter = Symbol("center");
    var htmlElem = $('#pedigree_data').html();
    var descentObj = JSON.parse(htmlElem);
    

    defineDimentions(descentObj, 0, 0);
    dimOfCanv.columns = dimOfCanv.goLeft + dimOfCanv.goRight;
    if (dimOfCanv.goLeft + dimOfCanv.goRight > 8) {
        canvas.height = 1500;
        canvas.width = 1500;
    }
    else if (dimOfCanv.goLeft + dimOfCanv.goRight > 6) {
        canvas.height = 1000;
        canvas.width = 1000;
    }
    else if (dimOfCanv.goLeft + dimOfCanv.goRight > 3) {
        canvas.height = 500;
        canvas.width = 500;
    }
    else {
        canvas.height = 250;
        canvas.width = 250;
    }
    var mr = 15; //margin on sides
    //if (dimOfCanv.goLeft + dimOfCanv.goRight <= 2) mr = 30;
    var mrT = 10; //margin for text
    var mrC = canvas.width / (dimOfCanv.goLeft + dimOfCanv.goRight) * 0.2;// margin btw columns
    var CSW = (canvas.width - mr * 2 - mrC * (dimOfCanv.columns - 2)) / (dimOfCanv.columns); // cell singe width
    var CSH = (canvas.height - mr * 2) / (dimOfCanv.rows + 1); // cell singe height
    ctx.strokeRect(0, 0, canvas.width, canvas.height);
    //console.log(dimOfCanv);
    //console.log("CSW=" + CSW + " CSH=" + CSH)
    var font = CSH * 0.2;
    ctx.font = font + 'px serif';
    ctx.strokeStyle = 'black';
    ctx.lineWidth = 2;
    var startPosCelX = (dimOfCanv.goLeft - 1) * (CSW + mrC) + mr;
    var startPosCelY = (dimOfCanv.rows) * CSH + CSH * 0.5 + mr + font + mrT;
    ctx.fillStyle = 'rgb(0, 0, 0)';
    ctx.fillText(descentObj.RabId, startPosCelX + CSW - mrT, startPosCelY);
    ctx.moveTo(startPosCelX + CSW - mrT + font / 2, startPosCelY - font);
    ctx.lineTo(startPosCelX + CSW - mrT + font / 2, (dimOfCanv.rows - 1) * CSH + CSH * 0.5 + mr + CSH - mrC, mrC / 2);
    ctx.stroke();

    drawCell(descentObj, startPosCelX, DrawCenter);
    function drawCell(decs, argX, DirectionToDraw) {

        if (decs.hasOwnProperty('Mother')) {
            var Y = (dimOfCanv.rows - decs.Step - 1) * CSH + mr + CSH * 0.5;
            var X = argX;
            //ctx.strokeStyle = 'red';
            //ctx.strokeRect(X, Y, CSW, CSH);
            ctx.fillStyle = 'rgb(230, 0, 0)';
            ctx.fillText(decs.Mother.RabId, X + mrT, Y + mrT + font * 1.3);
            if (decs.Mother.hasOwnProperty('Breed')) ctx.fillText(decs.Mother.Breed, X, Y + mr, CSW + CSW - mrT);

            ctx.moveTo(X + mrT + font / 2, Y + mrT + font * 1.6);
            //ctx.lineTo(X + mrT + font / 2, Y + CSH - mrC);
            ctx.arcTo(X + mrT + font / 2, Y + CSH - mrC, X + CSW, Y + CSH - mrC, mrC / 2);
            ctx.lineTo(X + CSW, Y + CSH - mrC);
            ctx.stroke();
            if (DirectionToDraw === DrawRight) {
                ctx.moveTo(X + mrT + CSW + mrC + font / 2, Y + CSH - mrC);
                ctx.lineTo(X + mrT + CSW + mrC + font / 2, Y + CSH + mrT);
                ctx.stroke();
            }
            //console.log(X + " " + Y + " Id:" + decs.Mother.RabId);
            var posCelX = X - CSW - mrC;
            var forwardDirection = DrawRight;
            drawCell(decs.Mother, posCelX, forwardDirection);
        }
        if (decs.hasOwnProperty('Father')) {
            var Y = (dimOfCanv.rows - decs.Step - 1) * CSH + mr + CSH * 0.5;
            var X = argX;

            //ctx.strokeStyle = 'blue';
            //ctx.strokeRect(X + CSW, Y, CSW, CSH);
            ctx.fillStyle = 'rgb(0, 0, 179)';

            if (decs.Father.hasOwnProperty('Breed')) {
                ctx.fillText(decs.Father.RabId, X + CSW * 2 - mrT - font, Y + mrT + font * 2.5);
                var breedOffset = 0;

                if (decs.Father.Breed.length < (CSW + CSW - mrT - font * 2) / (font * 0.5)) breedOffset = (CSW + CSW - mrT - font * 2) / 2;
                ctx.fillText(decs.Father.Breed, X + mrT + font * 2 + breedOffset, Y + mr + font * 1.3, CSW + CSW - mrT - font * 2 - breedOffset);
                ctx.moveTo(X + CSW * 2 - mrT - font / 2, Y + mrT + font * 2.8);
            } else {
                ctx.fillText(decs.Father.RabId, X + CSW * 2 - mrT - font, Y + mrT + font);
                ctx.moveTo(X + CSW * 2 - mrT - font / 2, Y + mrT + font * 1.3);
            }
            //ctx.lineTo(X + CSW * 2 - mrT-font/2, Y + CSH - mrC);
            ctx.arcTo(X + CSW * 2 - mrT - font / 2, Y + CSH - mrC, X + CSW, Y + CSH - mrC, mrC / 2)
            ctx.lineTo(X + CSW, Y + CSH - mrC);
            ctx.stroke();
            if (DirectionToDraw === DrawLeft) {
                ctx.moveTo(X + CSW - mrT - mrC - font / 2, Y + CSH - mrC);
                ctx.lineTo(X + CSW - mrT - mrC - font / 2, Y + CSH + mrT);
                ctx.stroke();
            }
            // console.log(X + " " + Y + " Id:" + decs.Father.RabId);
            var posCelY = parseInt(dimOfCanv.rows - decs.Father.Step - 1) * CSH + mr;
            var posCelX = X + CSW + mrC;
            var forwardDirection = DrawLeft;
            drawCell(decs.Father, posCelX, forwardDirection);
        }

    }





    function defineDimentions(decs, left, right) {

        if (dimOfCanv.rows < decs.Step) dimOfCanv.rows = decs.Step;
        if (decs.hasOwnProperty('Mother')) {
            dimOfCanv.columns += 1;
            var stepLeft = left + 1;
            var stepRight = right - 1;
            if (stepLeft > dimOfCanv.goLeft) dimOfCanv.goLeft = stepLeft;
            defineDimentions(decs.Mother, stepLeft, stepRight);
        }

        if (decs.hasOwnProperty('Father')) {
            var stepRight = right + 1;
            var stepLeft = left - 1;
            if (stepRight > dimOfCanv.goRight) dimOfCanv.goRight = stepRight;
            dimOfCanv.columns += 1;
            defineDimentions(decs.Father, stepLeft, stepRight)
        }
    }
}