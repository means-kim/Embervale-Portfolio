const fs=require('fs'),path=require('path');
const {createCanvas,loadImage}=require('C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@napi-rs/canvas');
async function main(){const file=path.resolve(__dirname,'../../../Assets/04.Sprites/NewPlayer/Design_v01/Player_Idle_Master_v01.png');const img=await loadImage(file),c=createCanvas(1024,1024),ctx=c.getContext('2d');for(let i=0;i<4;i++)ctx.drawImage(img,i%2*512,Math.floor(i/2)*512);fs.writeFileSync(path.join(__dirname,'Idle_FourFrame_Template.png'),c.toBuffer('image/png'));}main();
