
# Load data
data = read.csv('QuestData_Metrics2.csv')
View(data)

# Convert to nominal factor 
data$PtxID = factor(data$PtxID)
data$TrialNum = factor(data$TrialNum)
data$tempos = factor(data$tempos)
data$height = factor(data$height)
data$TargetID = factor(data$TargetID)

summary(data)

install.packages("plyr")
library(plyr)

# Explore data in light of EndError metric 
ddply(data, ~ height * TargetID, function(data) summary(data$EndError_cleaned))
ddply(data, ~ height * TargetID, summarise, EndError_cleaned.mean = mean(EndError_cleaned), sd = sd(EndError_cleaned))

# Histograms of data
hist(data[data$height == "low" & data$tempos == "80" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "low" & data$tempos == "120" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "low" & data$tempos == "160" & data$TargetID == "row_A1",]$EndError_cleaned)

hist(data[data$height == "mid" & data$tempos == "80" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "mid" & data$tempos == "120" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "mid" & data$tempos == "160" & data$TargetID == "row_A1",]$EndError_cleaned)

hist(data[data$height == "high" & data$tempos == "80" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "high" & data$tempos == "120" & data$TargetID == "row_A1",]$EndError_cleaned)
hist(data[data$height == "high" & data$tempos == "160" & data$TargetID == "row_A1",]$EndError_cleaned)

# Histograms for path offset 
hist(data[data$height == "low" & data$tempos == "80" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "low" & data$tempos == "120" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "low" & data$tempos == "160" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)

hist(data[data$height == "mid" & data$tempos == "80" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "mid" & data$tempos == "120" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "mid" & data$tempos == "160" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)

hist(data[data$height == "high" & data$tempos == "80" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "high" & data$tempos == "120" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)
hist(data[data$height == "high" & data$tempos == "160" & data$TargetID == "row_A1",]$PathOffsetNoLag_cleaned)

#height_reorder <- with(data, reorder(data$height=="low", data$height=="mid", data$height=="high", FUN=mean))

#boxplot(EndError_cleaned ~ height, data=data, xlab="height.tempos", ylab="EndError_cleaned")

# Reorder box plot height values 
data$height2 <- factor(data$height, levels = c("low", "mid", "high"))
data$tempos2 <- factor(data$tempos, levels = c("80", "120", "160"))
data$TargetID2 <- factor(data$TargetID, levels = c("row_A1", "row_A2", "row_A3","row_A4", "row_A5", "row_A6",
                                                  "row_B1", "row_B2", "row_B3","row_B4", "row_B5", "row_B6",
                                                  "row_C1", "row_C2", "row_C3","row_C4", "row_C5", "row_C6"))

boxplot(data$EndError_cleaned ~ data$height)
boxplot(data$EndError_cleaned ~ data$TargetID)
boxplot(data$EndError_cleaned ~ data$tempos)
with(data, interaction.plot(height2,tempos2, EndError_cleaned))
with(data, interaction.plot(TargetID2, height2, EndError_cleaned))

#install.packages("statmod")
#install.packages("lme4")
#install.packages("lmerTest")
#install.packages("car")
library(lme4)
library(lmerTest)
library(car)

# Set sum-to-zero contrasts for the Anova cells 
contrasts(data$tempos) <- "contr.sum"
contrasts(data$height) <- "contr.sum"
contrasts(data$TargetID) <- "contr.sum"
contrasts(data$TrialNum) <- "contr.sum"


# LMM order effect test
# Subject is a random effect 
#m = lmer(EndError_cleaned ~ (tempos * height * TargetID)/TrialNum + (1|PtxID), data=data)
#m = lmer(EndError_cleaned ~ (tempos * height * TargetID) + (1|height:tempos:TargetID:TrialNum) + (1|PtxID), data=data)

# -------------------------------------------------------------------------------------------------
# -------------------------------------- Positional Error -----------------------------------------
# -------------------------------------------------------------------------------------------------
m = glmer(EndError_cleaned ~ (TargetID * height) + (1|PtxID), data=data)
Anova(m, type=3, test.statistic = "F")

#summary(m)

# Perform post hoc pairwise comparisons 
#install.packages("multcomp")
#install.packages("lsmeans")

library(multcomp)
library(lsmeans)

# Positional Error post hoc analysis
summary(glht(m, lsm(pairwise ~ TargetID * height)), test=adjusted(type="holm"))
with(data, interaction.plot(TargetID, height, EndError_cleaned))

# -------------------------------------------------------------------------------------------------
# -------------------------------------- Path Offset ----------------------------------------------
# -------------------------------------------------------------------------------------------------

m2 = glmer(PathOffsetNoLag_cleaned ~ (TargetID * tempos * height) + (1|PtxID), data=data)
Anova(m2, type=3, test.statistic = "F")

# Path offset post hoc analysis
summary(glht(m2, lsm(pairwise ~ TargetID * tempos)), test=adjusted(type="holm"))
with(data, interaction.plot(TargetID, height, EndError_cleaned))


# -------------------------------------------------------------------------------------------------
# -------------------------------------- Angular Error ----------------------------------------------
# -------------------------------------------------------------------------------------------------

dataAng = read.csv('AngularError.csv')
# Convert to nominal factor 
data$Ptx = factor(data$Ptx)
data$Tempo = factor(data$Tempo)
data$Joint = factor(data$Joint)
data$AngErr_NoLag = factor(data$AngErr_NoLag)
summary(dataAng)

m2 = glmer(AngErr_NoLag ~ (Tempo * Joint) + (1|Ptx), data=dataAng)
Anova(m2, type=3, test.statistic = "F")

# Path offset post hoc analysis
summary(glht(m2, lsm(pairwise ~ Tempo + Joint)), test=adjusted(type="holm"))
with(dataAng, interaction.plot(Tempo, Joint, AngErr_NoLag))



