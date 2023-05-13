import dash
from dash import dcc
from dash import html
import plotly.express as px
from dash.dependencies import Input, Output
import pandas as pd


external_stylesheets = ['https://codepen.io/chriddyp/pen/bWLwgP.css']
app = dash.Dash(__name__, external_stylesheets=external_stylesheets)
df1 = pd.read_csv('dataset/college-salaries/salaries-by-college-type.csv')
#fig1 = px.bar(df1, x="School Type", y="Starting Median Salary", color="School Name", barmode="group")
fig1=px.scatter(df1[df1['School Type']=='State'], x="Starting Median Salary", y="Mid-Career Median Salary",
                size="size",size_max = 15,color="School Name")
df2= pd.read_csv('dataset/college-salaries/degrees-that-pay-back.csv')
fig2=px.scatter(df2,x='Starting Median Salary',y='Mid-Career Median Salary',
                size='Percent change from Starting to Mid-Career Salary',color='Undergraduate Major')
df3=pd.read_csv('dataset/college-salaries/salaries-by-region.csv')
fig3=px.bar(df3,x='Region',y='Starting Median Salary',color='School Name',barmode='group')
app.layout = html.Div([
    html.H1(children='WHZ-lab4——Data Visualization', style={'color': 'gold','text-align':'center'}),
    html.H2(children='Fig1:Starting Median Salary and Mid-Career Median Salary of different school types'),
    html.P(children='Please select a school type'),
    html.Div(children=[
        dcc.Dropdown(
            id='school_type',
            options=[{'label': i, 'value': i} for i in df1['School Type'].unique()],
            value='State',
        ),
        dcc.Graph(
            id='graph1',
            figure=fig1,
        )
    ]),
    html.H2(children='Fig2:Percent change from Starting to Mid-Career Salary of different major'),
    html.Div(children=[
        dcc.Graph(
            id='graph2',
            figure=fig2
        )
    ]),
    html.H2(children='Fig3:Starting Median Salary of schools in different regions'),
    html.Div(children=[
        dcc.Graph(
            id='graph3',
            figure=fig3
        )
    ])
])
@app.callback(
    Output('graph1', 'figure'),
    Input('school_type', 'value'))
def update_figure1(selected_type):
    filtered_df = df1[df1["School Type"] == selected_type]
    fig = px.scatter(filtered_df, x="Starting Median Salary", y="Mid-Career Median Salary",
                     size="size", size_max = 15,color="School Name")
    fig.update_layout(transition_duration=100)
    return fig

if __name__ == '__main__':
    app.run_server(port=9999)
