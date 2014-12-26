% fileName = 'Experiment 1\experiment 1.csv';
% For Octave online Matlab. http://octave-online.net/
fileName = 'experiment_1.csv';
% Get the headers
fid = fopen(fileName, 'r');
columnNames = fgetl(fid);
columnNames = regexp(columnNames, ',', 'split');
fclose(fid);

% Get the numeric data
data = csvread(fileName, 1, 0);


% Let's plot AverageTestingSetClassificationRate on Y (column 2) against MutationRate (column 8) on X.
% NOTE: I used phony data!
yColumnIndex = 3;
xColumnIndex = 8;

yData = data(:, yColumnIndex);
xData = data(:, xColumnIndex);
[xDataSorted, ySortIdx] = sort(xData);
yDataSorted = yData(ySortIdx);

% Change numPlots if you'd like more plots on the same window.
numPlots = 1;
ax1 = subplot(numPlots, 1, 1);
% set(ax1, 'XTick', xTicks_MA);
% set(ax1, 'YTick', yTicks_MA);
plot(xDataSorted, yDataSorted, 'b-');

% Use this code if you want to plot more things on the y-axis.
% hold on;
% plot(xData, yData2, 'g-'); //g- means 'green line'
% plot(xData, yData3, 'r-'); //'red line'
% hold off;
title(sprintf('How %s varies with %s', columnNames{yColumnIndex}, columnNames{xColumnIndex}));
xlabel(columnNames{xColumnIndex});
ylabel(columnNames{xColumnIndex});
% h_legend = legend('Raw data', 'Short term moving average', 'Long term moving average', 'Location','northwest');
% set(h_legend,'FontSize',8);