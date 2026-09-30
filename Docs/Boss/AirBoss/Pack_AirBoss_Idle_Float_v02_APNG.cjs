// Lossless packaging of the 16 completed PNGs; no artwork interpolation or modification.
const fs=require('fs'),path=require('path');
const root=path.resolve(__dirname,'../../..');
const crcTable=Array.from({length:256},(_,n)=>{for(let k=0;k<8;k++)n=(n&1)?(0xedb88320^(n>>>1)):(n>>>1);return n>>>0;});
function crc(buf){let c=0xffffffff;for(const v of buf)c=crcTable[(c^v)&255]^(c>>>8);return (c^0xffffffff)>>>0;}
function chunk(name,data){const type=Buffer.from(name),size=Buffer.alloc(4),checksum=Buffer.alloc(4);size.writeUInt32BE(data.length);checksum.writeUInt32BE(crc(Buffer.concat([type,data])));return Buffer.concat([size,type,data,checksum]);}
function readPng(file){const b=fs.readFileSync(file),out=[];for(let p=8;p<b.length;){const n=b.readUInt32BE(p),type=b.toString('ascii',p+4,p+8),data=b.subarray(p+8,p+8+n);out.push({type,data});p+=n+12;}return out;}
const frames=Array.from({length:16},(_,i)=>readPng(path.join(root,'ArtSource/AirBoss/idle/frames/v02',`AirBoss_Idle_Float_v02_${String(i).padStart(2,'0')}.png`)));
const header=frames[0].find(c=>c.type==='IHDR').data;
if(header.readUInt32BE(0)!==1024||header.readUInt32BE(4)!==1024||header[8]!==8||header[9]!==6)throw Error('Expected 1024 RGBA8');
if(!frames.every(f=>f.find(c=>c.type==='IHDR').data.equals(header)))throw Error('Mismatched PNG formats');
const actl=Buffer.alloc(8);actl.writeUInt32BE(16);actl.writeUInt32BE(0,4);
const output=[Buffer.from([137,80,78,71,13,10,26,10]),chunk('IHDR',header),chunk('acTL',actl)];let sequence=0;
frames.forEach((f,i)=>{const ctrl=Buffer.alloc(26);ctrl.writeUInt32BE(sequence++);ctrl.writeUInt32BE(1024,4);ctrl.writeUInt32BE(1024,8);ctrl.writeUInt16BE(3,20);ctrl.writeUInt16BE(40,22);ctrl[24]=1;ctrl[25]=0;output.push(chunk('fcTL',ctrl));for(const c of f.filter(c=>c.type==='IDAT')){if(i===0)output.push(chunk('IDAT',c.data));else{const seq=Buffer.alloc(4);seq.writeUInt32BE(sequence++);output.push(chunk('fdAT',Buffer.concat([seq,c.data])));}}});
output.push(chunk('IEND',Buffer.alloc(0)));
const destination=path.join(__dirname,'AirBoss_Idle_Float_v02_loop.png');fs.writeFileSync(destination,Buffer.concat(output));
const check=readPng(destination);if(check.filter(c=>c.type==='fcTL').length!==16)throw Error('Frame count mismatch');
// Confirm each APNG compressed frame is byte-identical to its source PNG IDAT stream.
let index=-1,collected=[];for(const c of check){if(c.type==='fcTL'){index++;collected[index]=[];}else if(c.type==='IDAT')collected[index].push(c.data);else if(c.type==='fdAT')collected[index].push(c.data.subarray(4));}
frames.forEach((f,i)=>{if(!Buffer.concat(f.filter(c=>c.type==='IDAT').map(c=>c.data)).equals(Buffer.concat(collected[i])))throw Error('Frame payload changed');});
console.log(JSON.stringify({path:destination,frames:16,secondsPerFrame:.075,loopSeconds:1.2,bytes:fs.statSync(destination).size,losslessFramePayloads:true}));
