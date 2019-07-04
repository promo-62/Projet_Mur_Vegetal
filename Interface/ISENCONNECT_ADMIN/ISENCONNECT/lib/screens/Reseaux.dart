import 'package:flutter/material.dart';
import 'package:tweet_webview/tweet_webview.dart';
import 'package:webview_flutter/webview_flutter.dart';

class SocialPage extends StatelessWidget {

  @override
  Widget build(BuildContext ctxt) {
    return Scaffold(
      body : DefaultTabController(
        length: 2,
        child: Scaffold(
          appBar: AppBar(
            backgroundColor: Colors.red[700],
            title: TabBar(
            indicatorColor: Colors.white,
            tabs:[
              Tab(text: "Twitter"),
              Tab(text: 'Instagram'),
            ],
          ),
        ),
      body : TabBarView(
        physics: NeverScrollableScrollPhysics(),
        children : [
          TweetWebView.tweetUrl("https://twitter.com/isenlille"),
          WebView(
            initialUrl: ('https://www.instagram.com/isen.lille/?hl=fr'),
            javascriptMode: JavascriptMode.unrestricted,
            ),
          ]
          ),
        ),
      ),
    );
  }
}