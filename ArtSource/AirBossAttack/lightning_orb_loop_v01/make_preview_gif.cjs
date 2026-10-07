const sharp=require('C:/Users/k_ms9/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const path=require('path');
(async()=>{
 const source=path.join(__dirname,'Attack_LightningOrb_FlightLoop_AirBoss_16F_v01.png');
 const frames=[];
 for(let f=0;f<16;f++)frames.push(await sharp(source).extract({left:f%4*512+96,top:Math.floor(f/4)*512+96,width:320,height:320}).flatten({background:'#182030'}).resize(480,480).removeAlpha().raw().toBuffer());
 const raw=Buffer.concat(frames);
 await sharp(raw,{raw:{width:480,height:480*16,channels:3,pageHeight:480}}).gif({loop:0,delay:Array.from({length:16},(_,i)=>[60,60,70,60][i%4]),colours:256,dither:0.6}).toFile(path.join(__dirname,'flight_loop_16fps.gif'));
 console.log(JSON.stringify(await sharp(path.join(__dirname,'flight_loop_16fps.gif'),{animated:true}).metadata()));
})().catch(e=>{console.error(e);process.exit(1)});

