library(RColorBrewer)

df <- read.csv(sep=",", 
quote="\"", 
dec=".", 
fill=TRUE, 
comment.char="", 
file="E:/R.Bioinformatics/datasets/ppg2008.csv", 
header=FALSE)
row.names(df) <- df$Name  
df<-df[,-1]
df <- data.matrix(df)
library(gplots)
png(type=c("windows", "cairo", "cairo-png"), 
filename="E:/R.Bioinformatics/datasets/ppg2008.heatmap.tiff", 
width=4000, 
height=3000, 
units="px", 
pointsize=12, 
bg="white", 
res=NA, 
family="", 
restoreConsole=TRUE)

result <- heatmap.2(Rowv=TRUE, 
Colv=TRUE, 
col=rev(brewer.pal(10,"RdYlBu")), 
notecex=0, 
trace="none", 
srtCol=45, 
offsetRow=0, 
offsetCol=0, 
key=TRUE, 
keysize=1.5, 
density.info="none", 
symkey=FALSE, 
densadj=0, 
x=df, 
symm=FALSE, 
revC=TRUE, 
scale="column", 
na.rm=FALSE, 
margins=c(15,15), 
cexRow=2, 
cexCol=2)
dev.off()

